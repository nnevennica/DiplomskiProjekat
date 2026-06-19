using DiplomskiProjekat.Models.Auth;

namespace DiplomskiProjekat.Services.Auth
{
    public interface IJwtTokenService
    {
        string CreateToken(User user);
    }
}
