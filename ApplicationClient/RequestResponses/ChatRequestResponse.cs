using System.Net.Http.Json;
using ApplicationClient.Models;

namespace ApplicationClient.RequestResponses;

public class ChatRequestResponse
{
    public static async Task<IEnumerable<Message>?> GetAllMessages(string userId, string receiverId)
    {
        using var client = new HttpClient();
        var response =
            await client.GetFromJsonAsync<IEnumerable<Message>>(
                $"{DataConst.Host}/api/chat/messages/{userId}/{receiverId}");

        if (!response.Any())
        {
            Console.WriteLine("No message. Want you dialog with current user?");
        }

        return response;
    }

    public static async Task UpdateMessage(string messageId, string content)
    {
        using var client = new HttpClient();
        var result =Guid.Parse(messageId);
        var response =
            await client.PutAsJsonAsync(
                $"{DataConst.Host}/api/chat/messages", new { Id = result, Content = content });
        
        if(!response.IsSuccessStatusCode)
            Console.WriteLine("Sorry your message not found( ");
    }

    public static async Task DeleteMessage(string messageId)
    {
        using var client = new HttpClient();
        var result =Guid.Parse(messageId);
        var response =
            await client.DeleteAsync(
                $"{DataConst.Host}/api/chat/messages/{result}");
        
        if(!response.IsSuccessStatusCode)
            Console.WriteLine("Sorry your message not found( ");
    }
}