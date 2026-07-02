using IOrder.Domain.Entities.Enums;
using System;

namespace IOrder.Domain.Repositories.Product;

public record ProductSearchCriteria(
    int PageNumber,
    int PageSize,
    string? Name,
    decimal? Price,
    PriceFilterType? PriceFilter,
    Guid? StoreId,
    string? OrderBy,
    bool IsDescending
);
