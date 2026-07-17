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
    Task<IList<Entities.Product>> GetByIdsTracking(IList<Guid> ids);

    Task<PromotionPrice> CreatePromotion(PromotionPrice promotionPrice);

    Task<IList<PromotionPrice>> GetPromotionsToStartAsync(DateTime now, CancellationToken cancellationToken);
    Task<IList<PromotionPrice>> GetPromotionsToFinishAsync(DateTime now, CancellationToken cancellationToken);
}
