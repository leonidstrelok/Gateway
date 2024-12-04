using ChatService.API.Model;

namespace ChatService.API.BLL.Interfaces;

public interface IMessageService
{
    Task<Message> SendMessageAsync(string senderId, string receiverId, string content);
    Task SendGroupMessageAsync(string senderId, string groupName, string content);
    Task<IEnumerable<Message>> GetMessagesAsync(string userId, string otherUserId);
    Task<bool> UpdateMessageAsync(Guid messageId, string content);
    Task<bool> DeleteMessageAsync(Guid messageId);
    Task<Guid> GetGroupByName(string groupName);
    Task CreateGroup(string groupName);
}