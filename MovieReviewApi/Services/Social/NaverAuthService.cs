using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.User;
using MovieReviewApi.Services.Auth;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using MovieReviewApi.DTOs.Accounts.User;
using MovieReviewApi.DTOs.Accounts.Auth;
using MovieReviewApi.External.OAuth.Naver;

namespace MovieReviewApi.Services.Social
{
    public class NaverAuthService(HttpClient httpClient, AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository,
                                  IUserHistoryRepository userHistoryRepository, IUserSocialAccountRepository userSocialAccountRepository, IAuthService authService) : INaverAuthService
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

            _httpContextAccessor.HttpContext?.Session.SetString("NaverOAuthState", state);
            _httpContextAccessor.HttpContext?.Session.SetString("NaverReturnUrl", returnUrl);

            var clientId    = _configuration["Naver:ClientId"];
            var redirectUri = _configuration["Naver:RedirectUri"];

            var url = "https://nid.naver.com/oauth2.0/authorize" +
                      $"?response_type=code" +
                      $"&client_id={Uri.EscapeDataString(clientId!)}" +
                      $"&state={Uri.EscapeDataString(state)}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}";

            return url;
        }

        public async Task<LoginResponse> LoginAsync(string code, string state)
        {
            // 1. OAuth 검증
            var savedState = _httpContextAccessor.HttpContext?.Session.GetString("NaverOAuthState");
            Console.WriteLine($"저장된 State : {savedState}");
            Console.WriteLine($"받은 State : {state}");
            if (string.IsNullOrEmpty(savedState) || savedState != state)
            {
                throw new InvalidOperationException("네이버 OAuth State가 유효하지 않습니다.");
            }
            // 사용한 state 삭제
            _httpContextAccessor.HttpContext?.Session.Remove("NaverOAuthState");

            // 2. 네이버 Access Token 요청
            var clientId     = _configuration["Naver:ClientId"]!;
            var clientSecret = _configuration["Naver:ClientSecret"]!;

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://nid.naver.com/oauth2.0/token?grant_type=authorization_code&client_id={Uri.EscapeDataString(clientId)}&client_secret={Uri.EscapeDataString(clientSecret)}&code={Uri.EscapeDataString(code)}&state={Uri.EscapeDataString(state)}");

            var tokenResponse = await _httpClient.SendAsync(request);
            var tokenJson     = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"네이버 Token 요청 실패 : {tokenJson}");
            }

            var tokenData = JsonSerializer.Deserialize<NaverTokenResponse>(tokenJson);

            if (tokenData == null)
            {
                throw new InvalidOperationException("네이버 Token 응답을 처리하지 못햇습니다.");
            }
            if (!string.IsNullOrEmpty(tokenData.Error))
            {
                throw new InvalidCastException($"네이버 Token 요청 실패 : {tokenData.Error} - {tokenData.ErrorDescription}");
            }
            if (string.IsNullOrEmpty(tokenData.AccessToken))
            {
                throw new InvalidOperationException("네이버 Access Token을 가져오지 못했습니다.");
            }

            // 3. 네이버 사용자 정보 요청
            using var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://openapi.naver.com/v1/nid/me");

            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

            var userResponse = await _httpClient.SendAsync(userRequest);
            var userJson     = await userResponse.Content.ReadAsStringAsync();

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"네이버 사용자 정보 요청 실패 : {userJson}");
            }

            var naverUserInfo = JsonSerializer.Deserialize<NaverUserInfo>(userJson);

            if (naverUserInfo?.Response == null)
            {
                throw new InvalidOperationException("네이버 사용자 정보를 가져오지 못했습니다.");
            }

            var naverUser = naverUserInfo.Response;

            if (string.IsNullOrEmpty(naverUser.Id))
            {
                throw new InvalidOperationException("네이버 사용자 식별자(ID)를 가져오지 못했습니다.");
            }

            // 4. 기존 네이버 계정 확인
            var exists    = await _userSocialAccountRepository.FindByProviderUserIdAsync("NAVER", naverUser.Id!);

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
                    User = new UserResponse
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

            // 5. 생년월일 변환
            DateTime? birthDate = null;

            if (!string.IsNullOrEmpty(naverUser.BirthYear) && !string.IsNullOrEmpty(naverUser.Birthday))
            {
                if (DateTime.TryParse($"{naverUser.BirthYear}-{naverUser.Birthday}", out var parsedBirthDate))
                {
                    birthDate = parsedBirthDate;
                }
            }

            // 6. 신규 User 생성
            var now = DateTime.Now;

            var user = new UserEntity
            {
                UserId      = $"NAVER_{naverUser.Id}",
                UserName    = naverUser.Nickname ?? naverUser.Name ?? "네이버 사용자",
                Email       = naverUser.Email,
                PhoneNumber = naverUser.Mobile?.Replace("-",""),
                CreatedAt   = now,

                Gender      = naverUser.Gender,
                BirthDate   = birthDate,
                UpdatedAt   = now,
                StatusCode  = 100,
                Role        = "User"
            };

            // 7. Social Account 생성
            var socialAccount = new UserSocialAccountEntity
            {
                User            = user,
                Provider        = "NAVER",
                ProviderUserId  = naverUser.Id,
                CreatedAt       = now,
                ProfileImageUrl = naverUser.ProfileImage
            };

            // 8. User History 생성
            var history = new UserHistoryEntity
            {
                User       = user,
                ActionCode = 100, //가입
                StatusCode = 100,
                ChangedAt  = now,
                Memo       = "네이버 소셜 계정 회원가입"
            };

            // 9. DB 저장(트랜잭션)
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

            // 10. JWT + LoginResponse
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
