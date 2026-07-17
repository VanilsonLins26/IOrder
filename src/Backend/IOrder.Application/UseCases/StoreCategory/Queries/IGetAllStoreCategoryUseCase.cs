using IOrder.Communication.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.StoreCategory.Queries;

public interface IGetAllStoreCategoryUseCase
{
    Task<IList<StoreCategoryResponseDto>> Execute();
}

