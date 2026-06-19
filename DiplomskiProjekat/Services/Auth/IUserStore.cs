using DiplomskiProjekat.Models.Auth;

namespace DiplomskiProjekat.Services.Auth
{
    public interface IUserStore
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User> CreateAsync(User user);
    }
}
