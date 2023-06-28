//using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using pl_backend.Data;
using pl_backend.Hubs;
using pl_backend.DTO;
using pl_backend.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Mvc;

namespace pl_backend.Services
{
    public class ChatService
    {
        private HubConnection? hubConnect;
        private readonly ITokenService _tokenService;
        public ChatService(DataContext dataContext, ITokenService tokenService, IAsyncDisposable asyncDisposable, NavigationManager navManager)
        {
            _tokenService = tokenService;
        }
        private string messages = string.Empty;
        

        private async Task Connect()
        {
            User? currentUser = _tokenService.GetCurrentUser();
            string userId = currentUser.Id.ToString();
            hubConnect = new HubConnectionBuilder().WithUrl($"/chathub?id={userId}").Build();

            hubConnect.On<string, string>("GetThatMessage", (currentUser, message) =>
            {
                var msg = $"{(string.IsNullOrEmpty(currentUser) ? "" : currentUser + ": ")}{message}";
                if (!string.IsNullOrWhiteSpace(message))
                {
                    messages += msg + "\n";
                }
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
                await hubConnect.DisposeAsync();
            }
        }
    }
}
