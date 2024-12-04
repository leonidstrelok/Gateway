using AuthService.API.DAL.Context;
using AuthService.API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Interfaces;

namespace AuthService.API.DAL;

public static class ServiceCollectionExtensions
{
    public static void AddDalServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(p =>
        {
            p.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<IApplicationDbContext>(p => p.GetService<AppDbContext>()!);
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}