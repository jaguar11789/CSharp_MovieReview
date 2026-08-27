using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieReviewApi.Common.Responses;
using MovieReviewApi.Data;
using MovieReviewApi.DTOs.Accounts.Auth;
using MovieReviewApi.DTOs.Accounts.User;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviewApi.Services.Auth
{
    public class AuthService(AppDbContext context, IUserRepository userRepository, IUserHistoryRepository userHistoryRepository, IConfiguration configuration) : IAuthService
    {
        private readonly AppDbContext               _context               = context;
        private readonly PasswordHasher<UserEntity> _passwordHasher        = new();
        private readonly IUserRepository            _userRepository        = userRepository;
        private readonly IUserHistoryRepository     _userHistoryRepository = userHistoryRepository;
        private readonly IConfiguration             _configuration         = configuration;

        // 중복확인
        public async Task<bool> CheckUserIdAsync(string userId)
        {
            bool exists = await _userRepository.ExistsByUserIdAsync(userId);

            return !exists;
        }

        // 회원가입
        public async Task<ResultResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var exists = await _userRepository.ExistsByUserIdAsync(registerRequest.UserId);

            if (exists)
            {
                return new ResultResponse
                {
                    retVal = 1,
                    retMsg = "이미 존재하는 아이디입니다."
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.Now;

                var user = new UserEntity
                {
                    UserId        = registerRequest.UserId,
                    UserName      = registerRequest.UserName,
                    Email         = registerRequest.Email,
                    PhoneNumber   = registerRequest.PhoneNumber,
                    Gender        = registerRequest.Gender,

                    BirthDate     = registerRequest.BirthDate,
                    ZipCode       = registerRequest.ZipCode,
                    BaseAddress   = registerRequest.BaseAddress,
                    DetailAddress = registerRequest.DetailAddress,
                    CreatedAt     = now
                };
            
                user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.PasswordHash);

                var userHistory = new UserHistoryEntity
                {
                    User          = user,
                    ActionCode    = 100,
                    StatusCode    = user.StatusCode,
                    Memo          = "회원가입",
                    ChangedAt     = DateTime.Now
                };

                _userRepository.Add(user);
                _userHistoryRepository.Add(userHistory);

                await _userRepository.SaveChangeAsync();
                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "회원가입이 완료되었습니다."
                };
            } 
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine(ex.ToString());

                return new ResultResponse
                {
                    retVal = 1,
                    retMsg = "회원가입 중 오류가 발생했습니다."
                };
            }
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.FindByUserIdAsync(request.UserId);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return new LoginResponse
                {
                    RetVal = 400,
                    RetMsg = "아이디 또는 비밀번호가 올바르지 않습니다."
                };
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return new LoginResponse
                {
                    RetVal = 400,
                    RetMsg = "아이디 또는 비밀번호가 올바르지 않습니다."
                };
            }

            if (user.StatusCode != 100)
            {
                return new LoginResponse
                {
                    RetVal = user.StatusCode,
                    RetMsg = user.StatusCode switch
                    {
                        200 => "휴면 회원입니다.",
                        990 => "정지된 회원입니다.",
                        999 => "탈퇴한 회원입니다.",
                        900 => "사용할 수 없는 회원입니다.",
                        _   => "로그인할 수 없는 회원입니다."
                    }
                };
            }

            return new LoginResponse
            {
                RetVal = 0,
                RetMsg = "로그인 되었습니다.",

                Token = CreateToken(user),

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

                    CreatedAt     = user.CreatedAt,
                    Role          = user.Role
                }
            };
        }

        public string CreateToken(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserId", user.UserId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer            : _configuration["Jwt:Issuer"],
                audience          : _configuration["Jwt:Audience"],
                claims            : claims,
                expires           : DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}