using System.Net.Http.Json;

namespace ApplicationClient.RequestResponses;

public class AuthorizationRequestResponse
{
    public static async Task<string> AuthenticateAsync(string username, string password)
    {
        using var client = new HttpClient();
        var response = await client.PostAsJsonAsync($"{DataConst.Host}/api/auth/login",
            new { Username = username, Password = password });

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(
                "Sorry with server response with not successfully status( Please repeat through 5 or 10 minutes");
        }

        var result = await response.Content.ReadFromJsonAsync<GetToken>();
        return result.Token;
    }
}