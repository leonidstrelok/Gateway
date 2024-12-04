using ChatService.API.BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.API.BLL.Services;

public class ChatHub(IMessageService messageService) : Hub
{
    private static readonly Dictionary<string, string> OnlineUsers = new();
    private static readonly Dictionary<string, List<string>> GroupUsers = new();

    public async Task SendMessage(string senderId, string receiverId, string content)
    {
        var message = await messageService.SendMessageAsync(senderId, receiverId, content);
        if (OnlineUsers.TryGetValue(receiverId, out _))
        {
            await Clients.User(receiverId).SendAsync("ReceiveMessage", message.SenderId, message.Content);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveMessageError", "Recipient is offline.");
        }
    }

    public async Task SendMessageToGroup(string groupName, string senderId, string content)
    {
        await messageService.SendGroupMessageAsync(senderId, groupName, content);
        await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", groupName, content);
    }

    public async Task JoinGroup(string groupName)
    {
        if (!GroupUsers.TryGetValue(groupName, out var value))
        {
            value = new List<string>();
            GroupUsers[groupName] = value;
        }
        if (GroupUsers.TryGetValue(groupName, out _))
        {
            await messageService.CreateGroup(groupName);    
        }
        value.Add(Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Caller.SendAsync("GroupJoined", groupName);
        await Clients.Group(groupName)
            .SendAsync("ReceiveGroupMessage", "System", $"{Context.UserIdentifier} joined the group");
        
        
    }

    public async Task LeaveGroup(string groupName)
    {
        if (GroupUsers.TryGetValue(groupName, out var user))
        {
            user.Remove(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        await Clients.Caller.SendAsync("GroupLeft", groupName);
        await Clients.Group(groupName)
            .SendAsync("ReceiveGroupMessage", "System", $"{Context.UserIdentifier} left the group");
    }

    public Task<List<string>> GetUsersInGroup(string groupName)
    {
        return Task.FromResult(GroupUsers.TryGetValue(groupName, out var user) ? user : []);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
        {
            OnlineUsers[userId] = Context.ConnectionId;

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            Console.WriteLine($"{userId} подключился");

            await Clients.All.SendAsync("UserOnline", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
        {
            OnlineUsers.Remove(userId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            Console.WriteLine($"{userId} отключился");

            await Clients.All.SendAsync("UserOffline", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task<IEnumerable<string>> GetOnlineUsers()
    {
        return OnlineUsers.Keys;
    }

    public async Task GetOnlineGroups()
    {
        var groups = GroupUsers.Keys.ToList();
        await Clients.Caller.SendAsync("GetOnlineGroups", groups);
    }
}