using IOrder.Communication.Response;
using System;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Queries;

public interface IGetCategoryByIdUseCase
{
    Task<CategoryResponseDto> Execute(Guid id);
}

