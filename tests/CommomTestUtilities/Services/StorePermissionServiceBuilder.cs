using IOrder.Application.Services.StorePermission;
using IOrder.Domain.Entities;
using Moq;
using System;
using System.Threading.Tasks;

namespace CommomTestUtilities.Services;

public class StorePermissionServiceBuilder
{
    public static IStorePermissionService Build(Guid? storeId = null)
    {
        var mock = new Mock<IStorePermissionService>();

        mock.Setup(s => s.GetLoggedUserStoreIdAsync())
            .ReturnsAsync(storeId ?? Guid.NewGuid());

        mock.Setup(s => s.ValidateProductOwnershipAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        mock.Setup(s => s.ValidateStoreOwnerAsync(It.IsAny<Store>()))
            .Returns(Task.CompletedTask);

        mock.Setup(s => s.ValidateStoreOwnerAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        mock.Setup(s => s.ValidateCategoryOwnershipAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        return mock.Object;
    }
}
