using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using pl_backend.Data;
using pl_backend.Hubs;
using pl_backend.DTO;
using pl_backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace pl_backend.Services
{
    public class ChatService
    {
        private HubConnection? hubConnect;
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IAsyncDisposable _asyncDisposable;
        public ChatService(DataContext dataContext, ITokenService tokenService, IAsyncDisposable asyncDisposable)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _asyncDisposable = asyncDisposable; 
        }
        private string messages = string.Empty;
        

        private async Task Connect()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            string username = currentUser.FirstName + " " + currentUser.LastName;
            hubConnect = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri($"/chathub?username={username}")).Build();

            hubConnect.On<string, string>("GetThatMessage", (currentUser, message) =>
            {
                var msg = $"{(string.IsNullOrEmpty(currentUser) ? "" : user + ": ")}{message}";
                if (!string.IsNullOrWhiteSpace(message))
                {
                    messages += msg + "\n";
                }
                StateHasChanged();
            });

            await hubConnect.StartAsync();
        }

        private async Task Send()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            string username = currentUser.FirstName + " " + currentUser.LastName;
            string message = string.Empty;
            if (hubConnect != null)
            {
                await hubConnect.SendAsync("AddMessageToChat", username, message);
                message = string.Empty;
            }
        }

        private async Task HandleInput(KeyboardEventArgs args)
        {
            if (args.Key == null || args.Key.Equals("Enter"))
            {
                await Send();
            }
        }

        public bool IsConnected => hubConnect?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnect != null)
            {
                await _asyncDisposable.DisposeAsync(hubConnect);
            }
        }
    }
}
