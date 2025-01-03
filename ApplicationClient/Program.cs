using ApplicationClient.Auxiliary;
using ApplicationClient.RequestResponses;
using Microsoft.AspNetCore.SignalR.Client;

namespace ApplicationClient;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var token = string.Empty;
        var isFinishedGetToken = false;
        while (!isFinishedGetToken)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine();
            Console.WriteLine("1. Registration");
            Console.WriteLine("2. Authorization");
            var registerOrLogin = PromptForInput("Please select options: ");
            var username = PromptForInput("Enter your name: ");
            var password = PromptForInput("Enter your password: ");
            switch (registerOrLogin)
            {
                case "1":
                    var email = PromptForInput("Enter your email: ");
                    token = await AuthorizationRequestResponse.RegistrationAsync(username, password, email);
                    if (!string.IsNullOrEmpty(token)) isFinishedGetToken = true;
                    break;
                case "2":
                    token = await AuthorizationRequestResponse.AuthenticateAsync(username, password);
                    if (!string.IsNullOrEmpty(token)) isFinishedGetToken = true;
                    break;
            }
        }

        var userId = MainAuxiliary.ExtractUserIdFromToken(token);

        var connection = CreateHubConnectionAsync(token);

        Events(connection);

        var isRunning = true;

        await connection.StartAsync();
        Console.WriteLine("Connected to the chat server!");

        while (isRunning)
        {
            var option = GetAllOption();

            switch (option)
            {
                case "1":
                    await WorkWithMessage(userId, connection);
                    break;

                case "2":
                    var groupName = PromptForInput("Enter group name: ");
                    var groupMessage = PromptForInput("Enter your message: ");
                    await connection.InvokeAsync("SendMessageToGroup", groupName, userId, groupMessage);
                    break;

                case "3":
                    var onlineUsers = await MainAuxiliary.GetAllOnlineUsers(token, userId);
                    Console.WriteLine("\nOnline users:");
                    foreach (var user in onlineUsers)
                    {
                        Console.WriteLine(user);
                    }

                    break;

                case "4":
                    var groupToJoin = PromptForInput("Enter group name to join: ");
                    await connection.InvokeAsync("JoinGroup", groupToJoin);
                    break;

                case "5":
                    var groupToLeave = PromptForInput("Enter group name to leave: ");
                    await connection.InvokeAsync("LeaveGroup", groupToLeave);
                    break;
                case "6":
                    await connection.InvokeAsync("GetOnlineGroups");
                    break;

                case "7":
                    isRunning = false;
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        await connection.StopAsync();
        Console.WriteLine("Disconnected from the chat server.");
    }

    private static void Events(HubConnection connection)
    {
        connection.On<string, string>("ReceiveMessage",
            (senderId, message) => Console.WriteLine($"New message from {senderId}: {message}"));
        connection.On<string, string>("ReceiveGroupMessage",
            (senderId, message) => { Console.WriteLine($"Group message from {senderId}: {message}"); });
        connection.On<string>("UserOnline", userIdOnline => Console.WriteLine($"{userIdOnline} is connected"));
        connection.On<string>("UserOffline", userIdOffline => Console.WriteLine($"{userIdOffline} is offline"));

        connection.On<List<string>>("GetOnlineGroups", groupNames =>
        {
            foreach (var groupName in groupNames)
            {
                Console.WriteLine();
                Console.WriteLine($"Group name: {groupName}");
                Console.WriteLine();
            }
        });
    }

    private static async Task WorkWithMessage(string userId, HubConnection connection)
    {
        var recipientId = PromptForInput("Enter recipient ID: ");
        await MainAuxiliary.GetAllMessage(userId, recipientId);

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1. Update message");
        Console.WriteLine("2. Delete");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option: ");
        var option = Console.ReadLine();

        await GetOptionForMessage(option);

        Console.WriteLine();
        var privateMessage = PromptForInput("Enter your message: ");
        await connection.InvokeAsync("SendMessage", userId, recipientId, privateMessage);
    }

    private static string? GetAllOption()
    {
        Console.WriteLine("\nOptions:");
        Console.WriteLine("1. Send private message");
        Console.WriteLine("2. Send group message");
        Console.WriteLine("3. View online users");
        Console.WriteLine("4. Join group");
        Console.WriteLine("5. Leave group");
        Console.WriteLine("6. Get all groups");
        Console.WriteLine("7. Exit");
        Console.Write("Select an option: ");
        var option = Console.ReadLine();
        return option;
    }

    private static async Task GetOptionForMessage(string messageOption)
    {
        switch (messageOption)
        {
            case "1":
                var messageId = PromptForInput("Please enter message id: ");
                var newContent = PromptForInput("Please enter new message content: ");
                await ChatRequestResponse.UpdateMessage(messageId, newContent);
                break;
            case "2":
                var messageIdForRemove = PromptForInput("Please enter message id: ");
                await ChatRequestResponse.DeleteMessage(messageIdForRemove);
                break;
            case "3":
                break;
        }
    }


    private static string PromptForInput(string prompt)
    {
        Console.WriteLine(prompt);
        return Console.ReadLine();
    }

    private static HubConnection CreateHubConnectionAsync(string token)
    {
        return new HubConnectionBuilder()
            .WithUrl($"{DataConst.Host}/chatHub", options =>
            {
                options.Headers.Add("access_token", token);
                options.HttpMessageHandlerFactory = handler => handler;
            })
            .Build();
    }
}

public record GetToken(string Token);