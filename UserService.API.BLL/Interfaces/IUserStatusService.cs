using UserService.API.Models;

namespace UserService.API.BLL.Interfaces;

public interface IUserStatusService
{
    Task SetUserStatusAsync(string userId, bool isOnline);
    Task<UserStatus?> GetUserStatusAsync(string userId);
    Task<IEnumerable<UserStatus>> GetAllOnlineUsersAsync();
}