using Microsoft.AspNetCore.SignalR;

namespace OrderService.Hubs
{
    public class OrderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                Console.WriteLine("✅ CONNECTED: " + userId);

                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                    Console.WriteLine("✅ ADDED TO GROUP: " + userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 HUB ERROR: " + ex.Message);
            }

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("❌ Client disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}