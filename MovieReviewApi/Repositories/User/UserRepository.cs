using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;
using System.Threading.Tasks;

namespace MovieReviewApi.Repositories.User
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        // 해당 UserId 가진 사용자 존재하는지 확인 true, false 반환
        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Users.AnyAsync(x => x.UserId == userId);
        }

        // UserId가 일치하는 사용자 한 명 조회
        public async Task<UserEntity?> FindByUserIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // DB의 PK(Id)를 기준으로 사용자 조회
        public async Task<UserEntity?> FindByIdAsync(long id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        // DB에 추가하도록 EF Core에 등록 INSERT X
        public void Add(UserEntity user)
        {
            _context.Users.Add(user);
        }

        // EF Core가 추적하고 있던 변경사항 DB 반영
        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
