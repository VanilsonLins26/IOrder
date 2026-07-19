using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Review;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Services;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Review.Commands;

public interface ICreateReviewUseCase
{
    Task<ReviewResponseDto> ExecuteAsync(Guid orderId, CreateReviewRequestDto request);
}

public class CreateReviewUseCase : ICreateReviewUseCase
{
    private readonly IOrderReadOnlyRepository _orderRepository;
    private readonly IReviewReadOnlyRepository _reviewReadOnlyRepository;
    private readonly IReviewWriteOnlyRepository _reviewWriteOnlyRepository;
    private readonly IStoreWriteOnlyRepository _storeWriteRepository;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewUseCase(
        IOrderReadOnlyRepository orderRepository,
        IReviewReadOnlyRepository reviewReadOnlyRepository,
        IReviewWriteOnlyRepository reviewWriteOnlyRepository,
        IStoreWriteOnlyRepository storeWriteRepository,
        ILoggedUserService loggedUserService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _reviewReadOnlyRepository = reviewReadOnlyRepository;
        _reviewWriteOnlyRepository = reviewWriteOnlyRepository;
        _storeWriteRepository = storeWriteRepository;
        _loggedUserService = loggedUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewResponseDto> ExecuteAsync(Guid orderId, CreateReviewRequestDto request)
    {
        Validate(request);

        var loggedUserId = _loggedUserService.GetUserId();

        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException(new List<string> { "Pedido não encontrado." });

        if (order.UserId != loggedUserId)
            throw new ErrorOnValidationException(new List<string> { "Apenas o cliente que fez o pedido pode avaliá-lo." });

        if (order.Status != OrderStatus.Delivered)
            throw new ErrorOnValidationException(new List<string> { "O pedido deve estar Entregue para ser avaliado." });

        var existingReview = await _reviewReadOnlyRepository.GetByOrderIdAsync(orderId);
        if (existingReview != null)
            throw new ErrorOnValidationException(new List<string> { "Este pedido já foi avaliado." });

        var activeAssignment = order.ActiveAssignment;
        string? courierUserId = activeAssignment?.Status == AssignmentStatus.Delivered ? activeAssignment.CourierUserId : null;

        var review = new OrderReview(
            orderId: order.Id,
            storeId: order.StoreId,
            userId: loggedUserId,
            courierUserId: courierUserId,
            storeRating: request.StoreRating,
            courierRating: request.CourierRating,
            comment: request.Comment
        );

        await _reviewWriteOnlyRepository.AddAsync(review);

        var store = await _storeWriteRepository.GetByIdTracking(order.StoreId);
        if (store != null)
        {
            store.AddRating(request.StoreRating);
        }

        await _unitOfWork.Commit();

        return review.Adapt<ReviewResponseDto>();
    }

    private void Validate(CreateReviewRequestDto request)
    {
        var validator = new CreateReviewValidator();
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

public class CreateReviewValidator : AbstractValidator<CreateReviewRequestDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.StoreRating)
            .InclusiveBetween(1, 5).WithMessage("A nota da loja deve ser entre 1 e 5.");
            
        RuleFor(x => x.CourierRating)
            .InclusiveBetween(1, 5).WithMessage("A nota do entregador deve ser entre 1 e 5.")
            .When(x => x.CourierRating.HasValue);
    }
}
