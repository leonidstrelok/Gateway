using System.Security.Claims;
using ChatService.API.BLL.Interfaces;
using MessageBroker.Configurations;
using MessageBroker.Interfaces;
using MessageBroker.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ChatService.API.BLL.Services;

public class ChatHub(IMessageService messageService, IEventPublisher eventPublisher, IOptions<KafkaConfig> options)
    : Hub
{
    private static readonly Dictionary<string, string> OnlineUsers = new();
    private static readonly Dictionary<string, List<string>> GroupUsers = new();

    public async Task SendMessage(string senderId, string receiverId, string content)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
            await Clients.Caller.SendAsync("ReceiveMessageError", "Произошла ошибка при отправке сообщения.");
        }
    }

    public async Task MarkMessageAsRead(string messageId)
    {
        var result = await messageService.MarkMessageAsRead(Guid.Parse(messageId));
        if (!result)
        {
            Console.WriteLine("Not found message Sorry");
            return;
        }

        await Clients.All.SendAsync("MessageRead", messageId, Context.UserIdentifier);
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
            var email = Context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email);
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            Console.WriteLine($"{userId} подключился");

            await Clients.All.SendAsync("UserOnline", userId);

            await eventPublisher.PublishAsync(options.Value.ChatUserServiceTopic,
                new UserStatusDto(userId, true, email.Value));
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
        {
            OnlineUsers.Remove(userId);
            var email = Context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            Console.WriteLine($"{userId} отключился");

            await Clients.All.SendAsync("UserOffline", userId);

            await eventPublisher.PublishAsync(options.Value.ChatUserServiceTopic,
                new UserStatusDto(userId, false, email.Value));
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