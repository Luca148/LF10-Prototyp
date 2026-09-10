using HarassmentFilter.Core.Services;
using Microsoft.AspNetCore.SignalR;

namespace LF10.Api.Hubs
{
    public class ChatHub(IHarassmentFilterService filter) : Hub
    {
        private static readonly Dictionary<string, (int chatRoomId, string username)> _connectedUsers = new();

        public async Task JoinRoom(int chatRoomId, string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
            _connectedUsers[Context.ConnectionId] = (chatRoomId, username);
            await BroadcastUserList(chatRoomId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectedUsers.TryGetValue(Context.ConnectionId, out var info))
            {
                _connectedUsers.Remove(Context.ConnectionId);
                await BroadcastUserList(info.chatRoomId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task BroadcastUserList(int chatRoomId)
        {
            var usersInRoom = _connectedUsers.Values
                .Where(u => u.chatRoomId == chatRoomId)
                .Select(u => u.username)
                .ToList();

            await Clients.Group(chatRoomId.ToString()).SendAsync("UserListUpdated", usersInRoom);
        }


        public async Task SendMessage(int chatRoomId, string content, string username)
        {
            var result = filter.FilterMessage(content);

            // TODO:
            // 1. current user from Claims
            // 2. check chatroom access
            // 3. save to MySQL
            // 4. only then broadcast

            await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", new 
            { 
                username,
                message = result.Message,
                wasModified = result.WasModified,
                harassmentTypes = result.HarassmentTypes,
                timestamp = result.Timestamp
            });

        }
    }
}