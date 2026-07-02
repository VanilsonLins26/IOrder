using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Exceptions.ExceptionBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Category.Commands;

public interface ICreateCategoryUseCase
{
    Task<CategoryResponseDto> Execute(CategoryRequestDto request);

    
}

