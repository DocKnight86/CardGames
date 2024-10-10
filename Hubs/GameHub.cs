using Microsoft.AspNetCore.SignalR;

namespace CardGames.Hubs
{
    public class GameHub : Hub
    {
        // Broadcast a move to all clients in the same game group
        public async Task SendMove(string gameId, string player, string move)
        {
            await Clients.Group(gameId).SendAsync("ReceiveMove", player, move);
        }

        // Called when a player joins a game, adds them to a SignalR group
        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        // Called when a player leaves a game, removes them from the SignalR group
        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}