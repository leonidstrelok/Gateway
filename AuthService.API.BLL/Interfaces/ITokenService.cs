using AuthService.API.Model;

namespace AuthService.API.BLL.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}