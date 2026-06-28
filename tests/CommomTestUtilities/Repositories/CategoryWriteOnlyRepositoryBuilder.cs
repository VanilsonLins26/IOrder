using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Category;
using Moq;
using System;

namespace CommomTestUtilities.Repositories;

public class CategoryWriteOnlyRepositoryBuilder
{
    private readonly Mock<ICategoryWriteOnlyRepository> _repository;

    public CategoryWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<ICategoryWriteOnlyRepository>();
    }



    public CategoryWriteOnlyRepositoryBuilder GetByIdTracking(Guid categoryId, Category? category)
    {
        _repository.Setup(repository => repository.GetByIdTracking(categoryId)).ReturnsAsync(category!);
        return this;
    }

    public CategoryWriteOnlyRepositoryBuilder Create()
    {
        _repository.Setup(x => x.Create(It.IsAny<Category>())).ReturnsAsync((Category category) => category);
        return this;
    }

    public CategoryWriteOnlyRepositoryBuilder Delete(Category category)
    {
        _repository.Setup(repository => repository.Delete(category));
        return this;
    }

    public ICategoryWriteOnlyRepository Build()
    {
        return _repository.Object;
    }
}
