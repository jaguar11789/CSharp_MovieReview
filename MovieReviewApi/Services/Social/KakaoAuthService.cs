using MovieReviewApi.Data;
using MovieReviewApi.DTOs.Accounts.Auth;
using MovieReviewApi.DTOs.Accounts.User;
using MovieReviewApi.External.OAuth.Kakao;
using MovieReviewApi.Models.Accounts;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.User;
using MovieReviewApi.Services.Auth;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;

namespace MovieReviewApi.Services.Social
{
    public class KakaoAuthService(HttpClient httpClient, AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository,
                                  IUserHistoryRepository userHistoryRepository, IUserSocialAccountRepository userSocialAccountRepository, IAuthService authService) : IKakaoAuthService
    {
        private readonly HttpClient                   _httpClient                  = httpClient;
        private readonly AppDbContext                 _context                     = context;
        private readonly IConfiguration               _configuration               = configuration;
        private readonly IHttpContextAccessor         _httpContextAccessor         = httpContextAccessor;

        private readonly IUserRepository              _userRepository              = userRepository;
        private readonly IUserHistoryRepository       _userHistoryRepository       = userHistoryRepository;
        private readonly IUserSocialAccountRepository _userSocialAccountRepository = userSocialAccountRepository;

        private readonly IAuthService _authService = authService;

        public string GetLoginUrl(string returnUrl)
        {
            var state = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            _httpContextAccessor.HttpContext?.Session.SetString("KakaoOAuthState", state);
            _httpContextAccessor.HttpContext?.Session.SetString("KakaoReturnUrl", returnUrl);

            var clientId    = _configuration["Kakao:RestApiKey"];
            var redirectUri = _configuration["Kakao:RedirectUri"];

            var url ="https://kauth.kakao.com/oauth/authorize" +
                      $"?client_id={Uri.EscapeDataString(clientId!)}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                      "&response_type=code" +
                      $"&state={Uri.EscapeDataString(state)}";

            return url;
        }

        public async Task<LoginResponse> LoginAsync(string code, string state)
        {
            // 1. OAuth 검증
            var savedState = _httpContextAccessor.HttpContext?.Session.GetString("KakaoOAuthState");

            if (string.IsNullOrEmpty(savedState) || savedState != state)
            {
                throw new InvalidOperationException("카카오 OAuth State가 유효하지 않습니다.");
            }
            // 사용한 state 삭제
            _httpContextAccessor.HttpContext?.Session.Remove("KakaoOAuthState");

            // 2. 인가 코드로 Access Token 요청
            var tokenResponse = await _httpClient.PostAsync("https://kauth.kakao.com/oauth/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"]    = "authorization_code",
                    ["client_id"]     = _configuration["Kakao:RestApiKey"]!,
                    ["redirect_uri"]  = _configuration["Kakao:RedirectUri"]!,
                    ["code"]          = code,
                    ["client_secret"] = _configuration["Kakao:ClientSecret"]!
                })
            );

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"카카오 Token 요청 실패: {tokenJson}");
            }

            var tokenData = JsonSerializer.Deserialize<KakaoTokenResponse>(tokenJson);
            
            if (tokenData == null)
            {
                throw new InvalidOperationException("카카오 Token 응답을 처리하지 못했습니다.");
            }

            if (string.IsNullOrEmpty(tokenData.AccessToken))
            {
                throw new InvalidOperationException("카카오 Access Token을 가져오지 못했습니다.");
            }

            // 3. Access Token으로 카카오 사용자 정보 요청
            using var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://kapi.kakao.com/v2/user/me");

            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

            var userResponse = await _httpClient.SendAsync(userRequest);
            var userJson     = await userResponse.Content.ReadAsStringAsync();

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"카카오 사용자 정보 요청 실패: {userJson}");
            }

            var kakaoUser = JsonSerializer.Deserialize<KakaoUserInfo>(userJson);
            Console.WriteLine("===== 카카오 사용자 정보 =====");
            Console.WriteLine(userJson);
            Console.WriteLine("=============================");
            if (kakaoUser == null)
            {
                throw new InvalidOperationException("카카오 사용자 정보를 가져오지 못했습니다.");
            }

            // 4. 기존 소셜 계정 확인
            var exists = await _userSocialAccountRepository.FindByProviderUserIdAsync("KAKAO", kakaoUser.Id.ToString());

            if (exists != null) 
            {
                var existsUser = await _userRepository.FindByIdAsync(exists.UserId);

                if (existsUser == null)
                {
                    throw new InvalidOperationException("소셜 계정에 연결된 회원 정보를 찾을 수 없습니다.");
                }

                return new LoginResponse
                {
                    Token = _authService.CreateToken(existsUser),
                    User  = new UserResponse
                    {
                        Id            = existsUser.Id,
                        UserId        = existsUser.UserId,
                        UserName      = existsUser.UserName,
                        Email         = existsUser.Email,
                        PhoneNumber   = existsUser.PhoneNumber,

                        Gender        = existsUser.Gender,
                        BirthDate     = existsUser.BirthDate,
                        ZipCode       = existsUser.ZipCode,
                        BaseAddress   = existsUser.BaseAddress,
                        DetailAddress = existsUser.DetailAddress,

                        CreatedAt     = existsUser.CreatedAt
                    }
                };
            }

            // 5. 신규 회원 생성
            var now  = DateTime.Now;
            var user = new UserEntity
            {
                UserId     = $"KAKAO_{kakaoUser.Id}",
                UserName   = kakaoUser.Account?.Profile?.Nickname ?? "카카오 사용자",
                CreatedAt  = now,
                UpdatedAt  = now,
                StatusCode = 100,

                Role       = "User",
            };

            // 6. 소셜 계정 생성
            var socialAccount = new UserSocialAccountEntity
            {
                User            = user,
                Provider        = "KAKAO",
                ProviderUserId  = kakaoUser.Id.ToString(),
                CreatedAt       = now,
                ProfileImageUrl = kakaoUser.Account?.Profile?.ProfileImageUrl,
            };

            // 7. history 생성
            var history = new UserHistoryEntity
            {
                User       = user,
                ActionCode = 100, // 가입
                StatusCode = 100, // 활성화
                ChangedAt  = now,
                Memo       = "카카오 소셜 계정 회원가입",
            };

            // 8. DB 저장(트랜잭션)
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _userSocialAccountRepository.Add(socialAccount);
                _userHistoryRepository.Add(history);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            // 9. JWT + LoginResponse
            return new LoginResponse
            {
                Token = _authService.CreateToken(user),
                User  = new UserResponse
                {
                    Id            = user.Id,
                    UserId        = user.UserId,
                    UserName      = user.UserName,
                    Email         = user.Email,
                    PhoneNumber   = user.PhoneNumber,

                    Gender        = user.Gender,
                    BirthDate     = user.BirthDate,
                    ZipCode       = user.ZipCode,
                    BaseAddress   = user.BaseAddress,
                    DetailAddress = user.DetailAddress,

                    CreatedAt     = user.CreatedAt
                }
            };
        }
    }
}
