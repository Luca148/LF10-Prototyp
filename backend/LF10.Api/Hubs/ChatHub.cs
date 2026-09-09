using HarassmentFilter.Core.Services;
using Microsoft.AspNetCore.SignalR;

namespace LF10.Api.Hubs
{
    public class ChatHub(IHarassmentFilterService filter) : Hub
    {
        public async Task SendMessage(int chatRoomId, string content)
        {
            var result = filter.FilterMessage(content);

            // TODO:
            // 1. current user from Claims
            // 2. check chatroom access
            // 3. save to MySQL
            // 4. only then broadcast

            await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", result);
        }
    }
}