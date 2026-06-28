using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Category;
using Moq;
using System;
using System.Collections.Generic;

namespace CommomTestUtilities.Repositories;

public class CategoryReadOnlyRepositoryBuilder
{
    private readonly Mock<ICategoryReadOnlyRepository> _repository;

    public CategoryReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<ICategoryReadOnlyRepository>();
    }

    public CategoryReadOnlyRepositoryBuilder NameExists(string name, Guid storeId, bool exists)
    {
        _repository.Setup(repository => repository.NameExists(name, storeId)).ReturnsAsync(exists);
        return this;
    }
    
    public CategoryReadOnlyRepositoryBuilder GetAll(Guid storeId, List<Category> categories)
    {
        _repository.Setup(repository => repository.GetAll(storeId)).ReturnsAsync(categories);
        return this;
    }

    public CategoryReadOnlyRepositoryBuilder GetByIdAsync(Guid categoryId, Category? category)
    {
        _repository.Setup(repository => repository.GetByIdAsync(categoryId)).ReturnsAsync(category!);
        return this;
    }

    public ICategoryReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}
