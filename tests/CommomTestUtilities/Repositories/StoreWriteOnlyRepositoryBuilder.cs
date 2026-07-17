using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Store;
using Moq;

namespace CommomTestUtilities.Repositories;

public class StoreWriteOnlyRepositoryBuilder
{
    private readonly Mock<IStoreWriteOnlyRepository> _repository;

    public StoreWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<IStoreWriteOnlyRepository>();
    }

    public StoreWriteOnlyRepositoryBuilder Create()
    {
        _repository.Setup(repository => repository.Create(It.IsAny<Store>()))
            .ReturnsAsync((Store store) => store);
        return this;
    }

    public StoreWriteOnlyRepositoryBuilder GetByIdTracking(Store store)
    {
        _repository.Setup(r => r.GetByIdTracking(It.IsAny<Guid>())).ReturnsAsync(store);
        return this;
    }

    public StoreWriteOnlyRepositoryBuilder Delete()
    {
        _repository.Setup(r => r.Delete(It.IsAny<Store>()));
        return this;
    }

    public IStoreWriteOnlyRepository Build() => _repository.Object;
}
