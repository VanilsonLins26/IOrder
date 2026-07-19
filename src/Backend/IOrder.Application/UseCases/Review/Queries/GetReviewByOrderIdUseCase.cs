using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Review;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Review.Queries;

public interface IGetReviewByOrderIdUseCase
{
    Task<ReviewResponseDto?> ExecuteAsync(Guid orderId);
}

public class GetReviewByOrderIdUseCase : IGetReviewByOrderIdUseCase
{
    private readonly IReviewReadOnlyRepository _reviewRepository;

    public GetReviewByOrderIdUseCase(IReviewReadOnlyRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewResponseDto?> ExecuteAsync(Guid orderId)
    {
        var review = await _reviewRepository.GetByOrderIdAsync(orderId);
        
        return review?.Adapt<ReviewResponseDto>();
    }
}
