using IOrder.Communication.Response;
using System;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Commands;

public interface IEmptyCategoryUseCase
{
    Task<CategoryResponseDto> Execute(Guid categoryId);
}

