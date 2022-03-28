

using Microsoft.AspNetCore.SignalR;

namespace dbChange
{
    public class SignalServer : Hub
    {
        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}