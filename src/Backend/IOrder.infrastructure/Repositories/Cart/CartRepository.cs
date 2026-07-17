using IOrder.Domain.Repositories.Cart;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IOrder.infrastructure.Repositories.Cart;

internal class CartRepository : ICartReadOnlyRepository, ICartWriteOnlyRepository
{

    private readonly IDistributedCache _redis;

    public CartRepository(IDistributedCache redis)
    {
        _redis = redis;
    }

    public async Task<bool> DeleteCartAsync(string userId)
    {
        await _redis.RemoveAsync($"cart:{userId}");

        return true;

    }

    public async Task<Domain.Entities.Cart?> GetCartAsync(string userId)
    {
        var cartJson = await _redis.GetStringAsync($"cart:{userId}");
        if (string.IsNullOrEmpty(cartJson))
            return null;

        return JsonSerializer.Deserialize<Domain.Entities.Cart>(cartJson);
    }

    public async Task<Domain.Entities.Cart> SaveCartAsync(Domain.Entities.Cart cart)
    {
        cart.LastModifiedAt = DateTime.UtcNow;

        var cartJson = JsonSerializer.Serialize(cart);

        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(7));

        await _redis.SetStringAsync($"cart:{cart.UserId}", cartJson, options);

        return cart;
    }
}
