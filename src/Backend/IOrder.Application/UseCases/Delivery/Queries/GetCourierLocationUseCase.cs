using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Exceptions;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Queries;

public interface IGetCourierLocationUseCase
{
    Task<CourierLocationResponseDto?> Execute(string courierUserId);
}

public class GetCourierLocationUseCase : IGetCourierLocationUseCase
{
    private readonly ICourierLocationReadOnlyRepository _readOnlyRepository;

    public GetCourierLocationUseCase(ICourierLocationReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<CourierLocationResponseDto?> Execute(string courierUserId)
    {
        var location = await _readOnlyRepository.GetByCourierUserIdAsync(courierUserId);
        return location?.Adapt<CourierLocationResponseDto>();
    }
}
