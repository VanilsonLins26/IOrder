using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category;

public interface IUpdateCategoryUseCase
{
    Task<CategoryResponseDto> Execute(Guid id, CategoryRequestDto request);
}
