using System;

namespace IOrder.Domain.Repositories.Store;

public record StoreSearchCriteria(
    int PageNumber,
    int PageSize,
    string? Name,
    Guid? CategoryId,
    string? OrderBy,
    bool IsDescending
);
