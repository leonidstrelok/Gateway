using AuthService.API.Model;
using AuthService.API.Model.DTOs;

namespace AuthService.API.BLL.Interfaces;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<User?> GetUserByUsernameAsync(string username);
}