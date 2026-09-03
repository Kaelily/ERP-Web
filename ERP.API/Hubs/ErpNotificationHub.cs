using Microsoft.AspNetCore.SignalR;

namespace ERP.API.Hubs;

public class ErpNotificationHub : Hub
{
    public async Task SendNotification(string user, string message, string type)
    {
        await Clients.All.SendAsync("ReceiveNotification", user, message, type, DateTime.UtcNow);
    }

    public async Task BroadcastMailingUpdate(int mailingId, string action)
    {
        await Clients.Others.SendAsync("MailingUpdated", mailingId, action, DateTime.UtcNow);
    }
}
