using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Commands;

public interface IUpdateStoreImageUseCase
{
    Task<string> Execute(Stream fileStream, string fileName);
}
