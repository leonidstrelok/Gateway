using System.IdentityModel.Tokens.Jwt;
using ApplicationClient.Models;
using ApplicationClient.RequestResponses;

namespace ApplicationClient.Auxiliary;

public static class MainAuxiliary
{
    public static async Task GetAllMessage(string userId, string recipientId)
    {
        var allMessages = await ChatRequestResponse.GetAllMessages(userId, recipientId);
        foreach (var message in allMessages)
        {
            Console.WriteLine();
            Console.WriteLine($"Id message: {message.Id}");
            Console.WriteLine($"Time send {message.SentAt}");

            Console.WriteLine(message.SenderId == userId
                ? $"Sender: {message.SenderId}"
                : $"Receiver: {message.SenderId}");

            Console.WriteLine($"Message: {message.Content}");
            Console.WriteLine();
        }
    }

    public static string ExtractUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenResult = tokenHandler.ReadJwtToken(token);
        return tokenResult.Claims.First(p => p.Type == "sub").Value;
    }

    public static async Task<IEnumerable<GetAllOnlineUser>> GetAllOnlineUsers(string token, string userId)
    {
        var onlineUsers = await UserStatusRequestResponse.UserStatusAsync(token, MethodType.GetAllOnline, userId);
        Console.WriteLine("Online users:");
        foreach (var user in onlineUsers)
        {
            if (user.UserId != userId)
            {
                Console.WriteLine($"{user.UserId}: {user.IsOnline}");
            }
        }

        return onlineUsers;
    }
}