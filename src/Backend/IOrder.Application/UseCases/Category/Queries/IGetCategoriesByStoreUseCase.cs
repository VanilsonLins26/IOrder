using IOrder.Communication.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Queries;

public interface IGetCategoriesByStoreUseCase
{
    Task<IList<CategoryResponseDto>> Execute(Guid storeId);
}

