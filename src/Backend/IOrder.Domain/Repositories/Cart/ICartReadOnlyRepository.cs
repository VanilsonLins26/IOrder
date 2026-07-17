using IOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Cart;

public interface ICartReadOnlyRepository
{
    Task<Entities.Cart?> GetCartAsync(string userId);
}
