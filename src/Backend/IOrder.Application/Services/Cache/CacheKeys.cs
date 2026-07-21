namespace IOrder.Application.Services.Cache;

public static class CacheKeys
{
    public static string StoreById(Guid storeId) => $"store:{storeId}";
    public static string StoreByUserId(string userId) => $"store:user:{userId}";
    public static string ProductsByStore(Guid storeId) => $"store:{storeId}:products";
    public static string ProductById(Guid productId) => $"product:{productId}";
    public static string CategoriesByStore(Guid storeId) => $"store:{storeId}:categories";
    public static string DashboardMetrics(Guid storeId) => $"dashboard:{storeId}";
}
