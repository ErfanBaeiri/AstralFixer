using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.DataLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region DI to DBContext
        private readonly BugFixerDbContext _dbcontext;
        public UserRepository(BugFixerDbContext dbContext)
        {
            _dbcontext = dbContext;
        }
        #endregion


        public async Task<bool> IsEmailExistByEmailAsync(string email)
        {
            return await _dbcontext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(User user)
        {
            await _dbcontext.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetUserByActivationCode(string activationCode)
        {
            return _dbcontext.Users.FirstOrDefaultAsync(u => u.EmailActivationCode == activationCode);
        }

        public async Task UpdateUser(User user)
        {
            await Task.Run(() => _dbcontext.Users.Update(user));
            //_dbcontext.Users.Update(user);
        }

        public async Task<User?> GetUserByIdAsync(long id)
        {
            return await _dbcontext.Users
                .AsNoTracking() // Use AsNoTracking for read-only operations to improve performance
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDelete);
        }
    }
}
