using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Product;
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

    public void GetAll(IEnumerable<Product> products)
    {
        _repository.Setup(repository => repository.GetAll()).Returns(products);
    }

    public void GetByIdAsync(Guid productId)
    {
        var product = new Product { Id = productId };
        product.UpdatePrice(50000m);
        _repository.Setup(repository => repository.GetByIdAsync(productId)).ReturnsAsync(product);
    }

    public void GetByIdAsync(Product product)
    {
        _repository.Setup(repository => repository.GetByIdAsync(product.Id)).ReturnsAsync(product);
    }

    public void ExistsPromotionInDate(Guid ProductId, DateTime initialDate, DateTime finalDate)
    {
        _repository.Setup(repository => repository.ExistsPromotionInDate(ProductId, initialDate, finalDate)).ReturnsAsync(true);
    }

    public void GetAllPagFiltroPrecoAsync(ProductSearchCriteria filter)
    {
        var product1 = new Product { Id = Guid.NewGuid(), Name = "Pão" };
        var product2 = new Product { Id = Guid.NewGuid(), Name = "Queijo" };
       
        var products = new List<Product> { product1, product2 };

        _repository.Setup(repository => repository.GetAllPagFiltroPrecoAsync(It.IsAny<ProductSearchCriteria>())).ReturnsAsync((products, 2));
    }

    public void GetProductPricesByIds(Dictionary<Guid, decimal> prices)
    {
        _repository.Setup(repo => repo.GetProductPricesByIds(It.IsAny<List<Guid>>())).ReturnsAsync(prices);
    }
}
