using IOrder.Domain.Repositories.Review;
using Moq;

namespace CommomTestUtilities.Repositories;

public class ReviewWriteOnlyRepositoryBuilder
{
    private readonly Mock<IReviewWriteOnlyRepository> _repository;

    public ReviewWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<IReviewWriteOnlyRepository>();
    }

    public IReviewWriteOnlyRepository Build() => _repository.Object;
}
