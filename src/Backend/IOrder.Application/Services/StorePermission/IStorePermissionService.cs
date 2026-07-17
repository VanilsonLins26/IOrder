using IOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.Services.StorePermission;

public interface IStorePermissionService
{
    Task<Guid> GetLoggedUserStoreIdAsync();
    Task ValidateProductOwnershipAsync(Product product);
    Task ValidateCategoryOwnershipAsync(Category category);
    Task ValidateStoreOwnerAsync(Guid storeID);
    Task ValidateStoreOwnerAsync(Store store);
}
