using UserService.API.Models;

namespace UserService.API.BLL.Interfaces;

public interface IUserStatusService
{
    Task SetUserStatusAsync(string userId, bool isOnline, string email);
    Task<UserStatus?> GetUserStatusAsync(string userId);
    Task<IEnumerable<UserStatus>> GetAllOnlineUsersAsync();
    Task<IEnumerable<UserStatus>> GetAllOfflineUsersAsync();
}