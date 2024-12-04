using ChatService.API.BLL.Interfaces;
using ChatService.API.Model;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace ChatService.API.BLL.Services;

public class MessageService(IApplicationDbContext dbContext) : IMessageService
{
    public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content)
    {
        var message = new Message()
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await dbContext.SaveChangesAsync(message, SaveChangeType.Add);
        return message;
    }

    public async Task SendGroupMessageAsync(string senderId, string groupName, string content)
    {
        var groupId = await GetGroupByName(groupName);
        var message = new Message
        {
            SenderId = senderId,
            GroupId = groupId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await dbContext.SaveChangesAsync(message, SaveChangeType.Add);
    }

    public async Task CreateGroupAsync(string groupName)
    {
        var group = new Group()
        {
            GroupName = groupName
        };
        await dbContext.SaveChangesAsync(group, SaveChangeType.Add);
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(string userId, string otherUserId)
    {
        return await dbContext.Set<Message>()
            .Where(p =>
                (p.SenderId == userId && p.ReceiverId == otherUserId) ||
                (p.SenderId == otherUserId && p.ReceiverId == userId))
            .OrderBy(p => p.SentAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateMessageAsync(Guid messageId, string content)
    {
        var message = await dbContext.Set<Message>().FindAsync(messageId);
        if (message is null) return false;

        message.Content = content;
        await dbContext.SaveChangesAsync(message, SaveChangeType.Update);
        return true;
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId)
    {
        var message = await dbContext.Set<Message>().FindAsync(messageId);
        if (message is null) return false;

        await dbContext.SaveChangesAsync(message, SaveChangeType.Delete);
        return true;
    }

    public async Task<Guid> GetGroupByName(string groupName)
    {
        var result = await dbContext.Set<Group>().FirstOrDefaultAsync(p => p.GroupName == groupName);
        return result.Id;
    }

    public async Task CreateGroup(string groupName)
    {
        var group = new Group()
        {
            GroupName = groupName
        };
        await dbContext.SaveChangesAsync(group, SaveChangeType.Add);
    }
}