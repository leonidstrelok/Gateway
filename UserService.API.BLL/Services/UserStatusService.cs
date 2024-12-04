﻿using System.Text.Json;
using StackExchange.Redis;
using UserService.API.BLL.Interfaces;
using UserService.API.Models;

namespace UserService.API.BLL.Services;

public class UserStatusService(IConnectionMultiplexer redis) : IUserStatusService
{
    private readonly IDatabase _db = redis.GetDatabase();

    private const string UserStatusKey = "user_status";

    public async Task SetUserStatusAsync(string userId, bool isOnline)
    {
        var status = new UserStatus()
        {
            UserId = userId,
            IsOnline = isOnline,
            LastActivity = DateTime.UtcNow
        };

        var serializedStatus = JsonSerializer.Serialize(status);
        await _db.HashSetAsync(UserStatusKey, userId, serializedStatus);

        if (!isOnline)
        {
            await _db.KeyExpireAsync($"{UserStatusKey}:{userId}", TimeSpan.FromMinutes(15));
        }
    }

    public async Task<UserStatus?> GetUserStatusAsync(string userId)
    {
        var value = await _db.HashGetAsync(UserStatusKey, userId);
        return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<UserStatus>(value!);
    }

    public async Task<IEnumerable<UserStatus>> GetAllOnlineUsersAsync()
    {
        var statuses = await _db.HashGetAllAsync(UserStatusKey);
        return statuses
            .Where(p => !p.Value.IsNullOrEmpty)
            .Select(p => JsonSerializer.Deserialize<UserStatus>(p.Value!))
            .Where(p => p is not null && p.IsOnline)
            .ToList()!;
    }
}