using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Cart;

public interface ICartWriteOnlyRepository
{
    Task<Entities.Cart> SaveCartAsync(Entities.Cart cart);

    Task<bool> DeleteCartAsync(string userId);
}
