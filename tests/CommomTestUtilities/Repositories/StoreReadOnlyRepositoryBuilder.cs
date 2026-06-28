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
    
    public StoreReadOnlyRepositoryBuilder GetByIdAsync(Store store)
    {
        _repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(store);
        return this;
    }

    public StoreReadOnlyRepositoryBuilder GetAllPaged(IList<Store> stores)
    {
        var pagedList = new IOrder.Domain.Pagination.PagedList<Store>([.. stores], stores.Count, 1, 10);
        _repository.Setup(repository => repository.GetAllPaged(It.IsAny<IOrder.Domain.SeedWork.Pagination.StoreSearchQuery>())).ReturnsAsync(pagedList);
        return this;
    }

    public IStoreReadOnlyRepository Build() => _repository.Object;
}
