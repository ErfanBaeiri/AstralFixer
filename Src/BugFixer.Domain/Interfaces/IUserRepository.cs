using BugFixer.Domain.Entities.Account;

namespace BugFixer.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsEmailExistByEmailAsync(string email);

        Task CreateUserAsync(User user);

        Task UpdateUser(User user);

        Task SaveChangesAsync();

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByActivationCode(string activationCode);
        IQueryable<User> GetAllUsers();

        Task<User?> GetUserByIdAsync(long id);
    }
}
