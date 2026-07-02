using IOrder.Communication.Request;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Commands;

public interface IUpdateCategoryPositionsUseCase
{
    Task Execute(UpdateCategoryPositionsRequestDto request);
}

