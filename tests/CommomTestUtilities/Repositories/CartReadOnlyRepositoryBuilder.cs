using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Cart;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CartReadOnlyRepositoryBuilder
{
    private readonly Mock<ICartReadOnlyRepository> _repository;

    public CartReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<ICartReadOnlyRepository>();
    }

    public CartReadOnlyRepositoryBuilder GetCartAsync(Cart cart)
    {
        _repository.Setup(repo => repo.GetCartAsync(cart.UserId)).ReturnsAsync(cart);
        return this;
    }

    public ICartReadOnlyRepository Build() => _repository.Object;
}
