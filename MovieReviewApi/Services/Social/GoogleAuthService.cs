using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.User;
using MovieReviewApi.Services.Auth;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using MovieReviewApi.DTOs.Accounts.User;
using MovieReviewApi.DTOs.Accounts.Auth;
using MovieReviewApi.External.OAuth.Google;

namespace MovieReviewApi.Services.Social
{
    public class GoogleAuthService(HttpClient httpClient, AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository,
                                   IUserHistoryRepository userHistoryRepository, IUserSocialAccountRepository userSocialAccountRepository, IAuthService authService) : IGoogleAuthService
    {
        private readonly HttpClient                   _httpClient = httpClient;
        private readonly AppDbContext                 _context = context;
        private readonly IConfiguration               _configuration = configuration;
        private readonly IHttpContextAccessor         _httpContextAccessor = httpContextAccessor;

        private readonly IUserRepository              _userRepository = userRepository;
        private readonly IUserHistoryRepository       _userHistoryRepository = userHistoryRepository;
        private readonly IUserSocialAccountRepository _userSocialAccountRepository = userSocialAccountRepository;

        private readonly IAuthService _authService = authService;

        public string GetLoginUrl(string returnUrl)
        {
            var clientId    = _configuration["Google:ClientId"]!;
            var redirectUri = _configuration["Google:RedirectUri"]!;

            var state       = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var scope       = "openid profile email";

            // OAuth 요청 시 생성한 state 세션에 저장
            _httpContextAccessor.HttpContext?.Session.SetString("GoogleOAuthState", state);
            _httpContextAccessor.HttpContext?.Session.SetString("GoogleReturnUrl", returnUrl);

            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?scope={Uri.EscapeDataString(scope)}" +
                      $"&access_type=offline" +
                      $"&include_granted_scopes=true" +
                      $"&response_type=code" +
                      $"&state={Uri.EscapeDataString(state)}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&client_id={Uri.EscapeDataString(clientId)}";

            return url;
        }

        public async Task<LoginResponse> LoginAsync(string code, string state)
        {
            // 1. OAuth State 검증
            var savedState = _httpContextAccessor.HttpContext?.Session.GetString("GoogleOAuthState");

            if (string.IsNullOrEmpty(savedState) || savedState != state)
            {
                throw new InvalidOperationException("구글 OAuth State가 유효하지 않습니다.");
            }
            // 사용한 state 삭제
            _httpContextAccessor.HttpContext?.Session.Remove("GoogleOAuthState");

            // 2. Google Access Token 요청
            var clientId     = _configuration["Google:ClientId"]!;
            var clientSecret = _configuration["Google:ClientSecret"]!;
            var redirectUri  = _configuration["Google:RedirectUri"]!;

            var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["code"]          = code,
                        ["client_id"]     = clientId,
                        ["client_secret"] = clientSecret,
                        ["redirect_uri"]  = redirectUri,
                        ["grant_type"]    = "authorization_code"
                    }
                )
            );

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"구글 토큰 요청 실패 : {tokenJson}");
            }

            var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenJson);

            if (tokenData == null)
            {
                throw new InvalidOperationException("구글 토큰 응답을 처리하지 못했습니다.");
            }

            if (string.IsNullOrEmpty(tokenData.AccessToken))
            {
                throw new InvalidOperationException("구글 Access 토큰을 가져오지 못했습니다.");
            }

            // 3. Google userinfo 요청
            using var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://openidconnect.googleapis.com/v1/userinfo");

            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

            var userResponse = await _httpClient.SendAsync(userRequest);
            var userJson     = await userResponse.Content.ReadAsStringAsync();

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"구글 사용자 정보 요청 실패 : {userJson}");
            }

            var googleUser = JsonSerializer.Deserialize<GoogleUserInfo>(userJson);

            if (googleUser == null)
            {
                throw new InvalidOperationException("구글 사용자 정보를 가져오지 못했습니다.");
            }

            if (string.IsNullOrEmpty(googleUser.Sub))
            {
                throw new InvalidOperationException("구글 사용자 식별자(sub)를 가져오지 못했습니다.");
            }

            // 4. 기존 Google 소셜 계정 확인
            var exists = await _userSocialAccountRepository.FindByProviderUserIdAsync("GOOGLE", googleUser.Sub);

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
            var now = DateTime.Now;
            var user = new UserEntity
            {
                UserId     = $"GOOGLE_{googleUser.Sub}",
                UserName   = googleUser.Name ?? "Google 사용자",
                Email      = googleUser.Email,
                CreatedAt  = now,
                UpdatedAt  = now,

                StatusCode = 100,
                Role       = "User"
            };

            // 6. Google Social Account 생성
            var socialAccount = new UserSocialAccountEntity
            {
                User            = user,
                Provider        = "GOOGLE",
                ProviderUserId  = googleUser.Sub,
                CreatedAt       = DateTime.Now,
                ProfileImageUrl = googleUser.Picture
            };

            // 7. User History 생성
            var history = new UserHistoryEntity
            {
                User       = user,
                ActionCode = 100,
                StatusCode = 100,
                ChangedAt  = DateTime.Now,
                Memo       = "구글 소셜 계정 회원가입"
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
