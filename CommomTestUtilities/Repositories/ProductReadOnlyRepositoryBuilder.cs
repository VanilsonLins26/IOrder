using IOrder.Domain.Entities;
using IOrder.Domain.Pagination;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.SeedWork.Pagination;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommomTestUtilities.Repositories;

public class ProductReadOnlyRepositoryBuilder
{
    private readonly Mock<IProductReadOnlyRepository> _repository;

    public ProductReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IProductReadOnlyRepository>();
    }

    public IProductReadOnlyRepository Build() => _repository.Object;

    public void NameExists(string productName)
    {
        _repository.Setup(repository => repository.NameExists(productName)).ReturnsAsync(true);
    }

    public void GetByIdAsync(Guid productId)
    {
        var product = new Product { Id = productId, Price = 50000m};
        _repository.Setup(repository => repository.GetByIdAsync(productId)).ReturnsAsync(product);
    }

    public void ExistsPromotionInDate(Guid ProductId, DateTime initialDate, DateTime finalDate)
    {
        _repository.Setup(repository => repository.ExistsPromotionInDate(ProductId, initialDate, finalDate)).ReturnsAsync(true);
    }

    public void GetAllPagFiltroPrecoAsync(ProductSearchQuery filter)
    {
        var product1 = new Product { Id = Guid.NewGuid(), Name = "Pão" };
        var product2 = new Product { Id = Guid.NewGuid(), Name = "Queijo" };
       

        var products = new List<Product> { product1, product2 };

        var pagedList = new PagedList<Product>(products, count: 2, pageNumber: 1, pageSize: 10);

        _repository.Setup(repository => repository.GetAllPagFiltroPrecoAsync(filter)).ReturnsAsync(pagedList);
    }


}
