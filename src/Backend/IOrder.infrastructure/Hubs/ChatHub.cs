using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace IOrder.infrastructure.Hubs;

public class ChatHub : Hub
{
    private readonly IDistributedCache _cache;

    public ChatHub(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
    }

    public async Task LeaveOrderGroup(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
    }

    public async Task MarkOrderRead(string orderId)
    {
        var dedupKey = $"dedup:chat:{orderId}";
        await _cache.RemoveAsync(dedupKey);
    }
}
