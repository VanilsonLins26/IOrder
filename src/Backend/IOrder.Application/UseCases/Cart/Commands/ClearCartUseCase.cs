using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Security.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Cart.Commands;

public class ClearCartUseCase : IClearCartUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;

    public ClearCartUseCase(ILoggedUserService loggedUserService, ICartWriteOnlyRepository writeOnlyRepository)
    {
        _loggedUserService = loggedUserService;
        _writeOnlyRepository = writeOnlyRepository;
    }

    public async Task Execute()
    {
        var userId = _loggedUserService.GetUserId();

        await _writeOnlyRepository.DeleteCartAsync(userId);

    }
}
