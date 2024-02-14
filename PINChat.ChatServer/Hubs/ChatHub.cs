using Microsoft.AspNetCore.SignalR;
using PINChat.ChatServer.Models;

namespace PINChat.ChatServer.Hubs;

public class ChatHub : Hub
{
    private class User
    {
        public string? UserId { get; set; }
        public string? ConnectionId { get; set; }
    }

    private static readonly List<User> _users = new();

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"<> User {Context.ConnectionId} connected");

        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = _users.Find(u => u.ConnectionId == Context.ConnectionId);
        if (user != null)
        {
            _users.Remove(user);
            Console.WriteLine($"User {user.UserId} disconnected");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task Login(string userId)
    {
        try
        {
            _users.RemoveAll(_users => _users.UserId == userId);

            _users.Add(new User()
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId
            });

            Console.WriteLine($"<> Logged in user: {userId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task AddGroup(string groupId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task RemoveGroup(string groupId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task MessageSingle(string action, MessageModel message)
    {
        try
        {
            var recipient = _users.Find(user => user.UserId == message.TargetId);

            if (recipient is not null)
            {
                await Clients.Client(recipient.ConnectionId!).SendAsync("ReceiveMessage", action, message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task MessageGroup(string action, MessageModel message)
    {
        try
        {
            await Clients.OthersInGroup(message.TargetId!).SendAsync("ReceiveMessage", action, message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
