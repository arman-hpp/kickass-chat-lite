using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace KickassChat.Hubs
{
    public sealed class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var token = httpContext?.Request.Query["access_token"].FirstOrDefault();
            if (string.IsNullOrEmpty(token))
            {
                Context.Abort();
                return;
            }

            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decoded.Split("::");

                if (parts.Length != 3)
                    throw new Exception("Invalid token format");

                //Context.Items["user"] = parts[0];
                //Context.Items["room"] = parts[1];
            }
            catch
            {
                Context.Abort();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string room)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task SendMessage(string user, string message, string time, string room, string userId)
        {
            try
            {
                await Clients.Group(room).SendAsync("ReceiveMessage", user, message, time, userId);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
