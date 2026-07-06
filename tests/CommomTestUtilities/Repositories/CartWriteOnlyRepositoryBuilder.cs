using IOrder.Domain.Repositories.Cart;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CartWriteOnlyRepositoryBuilder
{
    private readonly Mock<ICartWriteOnlyRepository> _repository;

    public CartWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<ICartWriteOnlyRepository>();
        _repository.Setup(repository => repository.SaveCartAsync(It.IsAny<IOrder.Domain.Entities.Cart>()))
            .ReturnsAsync((IOrder.Domain.Entities.Cart cart) => cart);
        _repository.Setup(repository => repository.DeleteCartAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    public ICartWriteOnlyRepository Build() => _repository.Object;
}

