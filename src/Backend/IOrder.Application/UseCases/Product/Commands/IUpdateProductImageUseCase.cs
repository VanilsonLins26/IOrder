using System;
using System.IO;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Product.Commands;

public interface IUpdateProductImageUseCase
{
    Task<string> Execute(Guid productId, Stream fileStream, string fileName);
}
