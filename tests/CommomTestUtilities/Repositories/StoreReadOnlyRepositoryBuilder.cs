using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Store;
using Moq;

namespace CommomTestUtilities.Repositories;

public class StoreReadOnlyRepositoryBuilder
{
    private readonly Mock<IStoreReadOnlyRepository> _repository;

    public StoreReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IStoreReadOnlyRepository>();
    }

    public StoreReadOnlyRepositoryBuilder NameExists(string name)
    {
        _repository.Setup(repository => repository.NameExists(name)).ReturnsAsync(true);
        return this;
    }

    public StoreReadOnlyRepositoryBuilder HasStore(string userId)
    {
        _repository.Setup(repository => repository.HasStore(userId)).ReturnsAsync(true);
        return this;
    }

    public StoreReadOnlyRepositoryBuilder GetByUserIdAsync(Store store)
    {
        _repository.Setup(repository => repository.GetByUserIdAsync(store.UserId)).ReturnsAsync(store);
        return this;
    }

    public StoreReadOnlyRepositoryBuilder GetByUserIdAsyncReturnsNull()
    {
        _repository.Setup(repository => repository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((Store?)null);
        return this;
    }
    
    public StoreReadOnlyRepositoryBuilder GetByIdAsync(Store store)
    {
        _repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(store);
        return this;
    }

    public StoreReadOnlyRepositoryBuilder GetAllPaged(IList<Store> stores)
    {
        _repository.Setup(repository => repository.GetAllPaged(It.IsAny<StoreSearchCriteria>())).ReturnsAsync((stores, stores.Count));
        return this;
    }

    public IStoreReadOnlyRepository Build() => _repository.Object;
}
