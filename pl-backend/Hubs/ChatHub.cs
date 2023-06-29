using Microsoft.AspNetCore.SignalR;
using pl_backend.Data;
using pl_backend.Models;
using pl_backend.Services;

namespace pl_backend.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> Users = new Dictionary<string, string>();
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        public ChatHub(DataContext dataContext, ITokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }
        public override async Task OnConnectedAsync()
        {
            string username = "username";
            Users.Add(Context.ConnectionId, username);
            //await AddMessageToChat(string.Empty, $"{username} is here!");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            //await AddMessageToChat(string.Empty, $"{username} left!");
        }

        public async Task AddMessageToChat(string user, string message)
        {
            await Clients.All.SendAsync("GetThatMessage", user, message);
        }
    }
}
