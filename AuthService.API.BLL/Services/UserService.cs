using AuthService.API.BLL.Interfaces;
using AuthService.API.Model;
using AuthService.API.Model.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace AuthService.API.BLL.Services;

public class UserService(IApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    : IUserService
{
    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await dbContext.Set<User>().FirstOrDefaultAsync(p => p.Username == username);
        if (user is null) return null;

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        if (await dbContext.Set<User>()
                .AnyAsync(p => p.Username == request.Username || p.Email == request.Email)) return false;

        var user = new User()
        {
            Username = request.Username,
            Email = request.Email
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await dbContext.SaveChangesAsync(user, SaveChangeType.Add);
        return true;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await dbContext.Set<User>().FirstOrDefaultAsync(p => p.Username == username);
    }
}