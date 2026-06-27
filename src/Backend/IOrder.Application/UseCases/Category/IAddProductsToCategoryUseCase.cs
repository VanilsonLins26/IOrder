using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category;

public interface IAddProductsToCategoryUseCase
{
    Task<CategoryResponseDto> Execute(Guid categoryId, AddProductsToCategoryRequestDto request);
}
