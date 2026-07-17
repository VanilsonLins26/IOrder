using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.StoreCategory;
using Moq;
using System.Collections.Generic;

namespace CommomTestUtilities.Repositories;

public class StoreCategoryReadOnlyRepositoryBuilder
{
    private readonly Mock<IStoreCategoryReadOnlyRepository> _repository;

    public StoreCategoryReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IStoreCategoryReadOnlyRepository>();
    }

    public StoreCategoryReadOnlyRepositoryBuilder GetAllActive(IList<StoreCategory> categories)
    {
        _repository.Setup(repository => repository.GetAllActive()).ReturnsAsync(categories);
        return this;
    }

    public IStoreCategoryReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}
