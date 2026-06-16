using IOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Product;

public interface IProductWriteOnlyRepository
{
    Task <Entities.Product> Create(Entities.Product product);

    Entities.Product Delete(Entities.Product product);

    Entities.Product Update(Entities.Product product);

    Task<Entities.Product> GetByIdTracking(Guid id);

    Task<PromotionPrice> CreatePromotion(PromotionPrice promotionPrice);
}
