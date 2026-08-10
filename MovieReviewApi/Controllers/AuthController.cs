using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Services.Auth;
using MovieReviewApi.Services.Social;
using System.Security.Claims;
using System.Threading.Tasks;
using MovieReviewApi.DTOs.Accounts.Auth;
using MovieReviewApi.Common.Responses;

namespace MovieReviewApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IKakaoAuthService kakaoAuthService, INaverAuthService naverAuthService, IGoogleAuthService googleAuthService) : ControllerBase
    {
        private readonly IAuthService       _authService       = authService;
        private readonly IKakaoAuthService  _kakaoAuthService  = kakaoAuthService;
        private readonly INaverAuthService  _naverAuthService  = naverAuthService;
        private readonly IGoogleAuthService _googleAuthService = googleAuthService;

        // 중복확인
        [HttpGet("check-userId")]
        public IActionResult CheckUserId(string userId)
        {
            var available = _authService.CheckUserIdAsync(userId);

            return Ok(new { available });
        }

        // 회원가입
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            var result = _authService.RegisterAsync(request);

            return Ok(result);
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized(new ResultResponse
                {
                    retVal = 900,
                    retMsg = "아이디 또는 비밀번호가 올바르지 않습니다."
                });
            }

            Response.Cookies.Append("accessToken", result.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure   = true,
                    SameSite = SameSiteMode.None,
                    Expires  = DateTime.Now.AddMinutes(30)
                });
            return Ok(result);
        }

        // 카카오 로그인
        [HttpGet("kakao/login")]
        public IActionResult KakaoLogin(string returnUrl)
        {
            var result = _kakaoAuthService.GetLoginUrl(returnUrl);

            return Redirect(result);
        }

        [HttpGet("kakao/callback")]
        public async Task<IActionResult> KakaoCallback(string code, string state)
        {
            try
            {
                var result = await _kakaoAuthService.LoginAsync(code, state);

                Response.Cookies.Append("accessToken", result.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure   = true,
                        SameSite = SameSiteMode.None,
                        Expires  = DateTime.Now.AddMinutes(30)
                    });

                var returnUrl = HttpContext.Session.GetString("KakaoReturnUrl");

                return Redirect($"https://localhost:7185{returnUrl ?? "/index"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"카카오 로그인 실패: {ex.Message}");

                return Redirect("https://localhost:7185/login");
            }
        }

        // 네이버 로그인
        [HttpGet("naver/login")]
        public IActionResult NaverLogin(string returnUrl)
        {
            var url = _naverAuthService.GetLoginUrl(returnUrl);

            return Redirect(url);
        }

        [HttpGet("naver/callback")]
        public async Task<IActionResult> NaverCallback(string code, string state)
        {
            try
            {
                var result = await _naverAuthService.LoginAsync(code, state);

                Response.Cookies.Append("accessToken", result.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure   = true,
                        SameSite = SameSiteMode.None,
                        Expires  = DateTime.Now.AddMinutes(30)
                    });

                var returnUrl = HttpContext.Session.GetString("NaverReturnUrl");

                return Redirect($"https://localhost:7185{returnUrl ?? "/index"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"네이버 로그인 실패 : {ex.Message}");

                return Redirect("https://localhost:7185/login");
            }
        }

        // 구글 로그인
        [HttpGet("google/login")]
        public IActionResult GoogleLogin(string returnUrl)
        {
            var url = _googleAuthService.GetLoginUrl(returnUrl);
            
            return Redirect(url);
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback(string code, string state)
        {
            try
            {
                var result = await _googleAuthService.LoginAsync(code, state);

                Response.Cookies.Append("accessToken", result.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure   = true,
                        SameSite = SameSiteMode.None,
                        Expires  = DateTime.Now.AddMinutes(30)
                    });

                var returnUrl = HttpContext.Session.GetString("NaverReturnUrl");

                return Redirect($"https://localhost:7185{returnUrl ?? "/index"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"구글 로그인 실패: {ex.Message}");

                return Redirect("https://localhost:7185/login");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken");

            return Ok(new { message = "로그아웃" });
        }

        // 인증 확인용
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Ok(null);
            }
            var id       = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId   = User.FindFirst("UserId")?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var role     = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                id,
                userId,
                userName,
                role
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-test")]
        public IActionResult AdminTest()
        {
            return Ok("관리자 인증 성공");
        }
    }
}
