using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Review;
using Mapster;

namespace IOrder.Application.UseCases.Review.Queries;

public interface IGetStoreReviewsUseCase
{
    Task<IList<ReviewResponseDto>> ExecuteAsync(Guid storeId, int page, int pageSize);
}

public class GetStoreReviewsUseCase : IGetStoreReviewsUseCase
{
    private readonly IReviewReadOnlyRepository _reviewRepository;

    public GetStoreReviewsUseCase(IReviewReadOnlyRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IList<ReviewResponseDto>> ExecuteAsync(Guid storeId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 20;

        var reviews = await _reviewRepository.GetByStoreIdAsync(storeId, page, pageSize);

        return reviews.Adapt<IList<ReviewResponseDto>>();
    }
}
