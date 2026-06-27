using IOrder.Communication.Request;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category;

public interface IUpdateCategoryPositionsUseCase
{
    Task Execute(UpdateCategoryPositionsRequestDto request);
}
