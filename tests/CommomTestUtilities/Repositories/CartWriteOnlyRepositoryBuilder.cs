using IOrder.Domain.Repositories.Cart;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CartWriteOnlyRepositoryBuilder
{
    private readonly Mock<ICartWriteOnlyRepository> _repository;

    public CartWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<ICartWriteOnlyRepository>();
    }

    public ICartWriteOnlyRepository Build() => _repository.Object;
}
