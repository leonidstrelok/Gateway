using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApplicationClient.Models;

namespace ApplicationClient.RequestResponses;

public class UserStatusRequestResponse
{
    public static async Task<IEnumerable<GetAllOnlineUser>?> UserStatusAsync(string token, MethodType? method,
        string userId)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        switch (method)
        {
            case MethodType.Set:
                await client.PostAsJsonAsync($"{DataConst.Host}/api/status/set",
                    new { IsOnline = true, UserId = userId });
                break;
            case MethodType.GetById:
                var getById = await client.GetFromJsonAsync<GetAllOnlineUser>($"{DataConst.Host}/api/status/{userId}");
                var result = new List<GetAllOnlineUser>();
                result.Add(getById);
                return result;
            case MethodType.GetAllOnline:
                var getAllUsers =
                    await client.GetFromJsonAsync<IEnumerable<GetAllOnlineUser>>($"{DataConst.Host}/api/status/online");
                if(!getAllUsers.Any()) Console.WriteLine("Sorry now online users((");
                return getAllUsers;
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }

        return null;
    }
}