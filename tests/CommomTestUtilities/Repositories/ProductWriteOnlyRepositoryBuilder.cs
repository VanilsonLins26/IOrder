using CommomTestUtilities.Requests;
using IOrder.Communication.Request;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Product;
using Moq;

namespace CommomTestUtilities.Repositories;

public class ProductWriteOnlyRepositoryBuilder
{
    private readonly Mock<IProductWriteOnlyRepository> _repository;

    public ProductWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<IProductWriteOnlyRepository>();
    }

    public ProductWriteOnlyRepositoryBuilder Create()
    {
        _repository.Setup(x => x.Create(It.IsAny<Product>())).ReturnsAsync((Product product) => product);
        return this;
    }

    public IProductWriteOnlyRepository Build() => _repository.Object;

    public ProductWriteOnlyRepositoryBuilder CreatePromotion()
    {
        _repository.Setup(x => x.CreatePromotion(It.IsAny<PromotionPrice>())).ReturnsAsync((PromotionPrice promotion) => promotion);
        return this;
    }

    public void GetByIdTracking(Guid productId)
    {
        var product = new Product { Id = productId };
        product.UpdatePrice(50000m);
        _repository.Setup(repository => repository.GetByIdTracking(productId)).ReturnsAsync(product);
    }

    public void GetByIdsTracking(IList<Guid> productIds, IList<Product> products)
    {
        _repository.Setup(repository => repository.GetByIdsTracking(productIds)).ReturnsAsync(products);
    }

}
