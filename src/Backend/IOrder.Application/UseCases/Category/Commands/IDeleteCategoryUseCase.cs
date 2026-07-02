using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Category.Commands;

public interface IDeleteCategoryUseCase
{
    Task<CategoryResponseDto> Execute(Guid id);
}

