using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Product;
using IOrder.infrastructure.DataAccess;
using IOrder.infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IOrder.infrastructure.Repositories.Product;

internal class ProductRepository : IProductReadOnlyRepository, IProductWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Domain.Entities.Product> GetAll()
    {
        return _context.Products.AsNoTracking();
    }

    public async Task<Domain.Entities.Product> GetByIdAsync(Guid id)
    {
        return await _context.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<Domain.Entities.Product> GetByIdTracking(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<IList<Domain.Entities.Product>> GetByIdsTracking(IList<Guid> ids)
    {
        return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
    }



    public async Task<(IList<Domain.Entities.Product> Items, int TotalCount)> GetAllPagFiltroPrecoAsync(ProductSearchCriteria criteria)
    {
        var query = _context.Products.AsNoTracking();

        if (criteria.Price.HasValue && criteria.PriceFilter.HasValue)
        { 
            query = criteria.PriceFilter.Value switch
            {
                PriceFilterType.GreaterThan => query.Where(p => (p.CurrentPromotionalPrice ?? p.Price) > criteria.Price.Value),

                PriceFilterType.LessThan => query.Where(p => (p.CurrentPromotionalPrice ?? p.Price) < criteria.Price.Value),

                PriceFilterType.EqualTo => query.Where(p => (p.CurrentPromotionalPrice ?? p.Price) == criteria.Price.Value),

                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(criteria.Name))
            query = query.Where(p => p.Name.Contains(criteria.Name));

        if (criteria.StoreId.HasValue)
            query = query.Where(p => p.StoreId == criteria.StoreId.Value);

        var property = criteria.OrderBy?.ToLower().Trim();

        query = property switch
        {
            "name" => criteria.IsDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            "price" => criteria.IsDescending
                ? query.OrderByDescending(p => p.CurrentPromotionalPrice ?? p.Price)
                : query.OrderBy(p => p.CurrentPromotionalPrice ?? p.Price),

            _ => criteria.IsDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
        };

        return await query.ToPaginatedTupleAsync(criteria.PageNumber, criteria.PageSize);
    }



    public async Task<Domain.Entities.Product> Create(Domain.Entities.Product product)
    {
        await _context.Products.AddAsync(product);
        return product;
    }

    public Domain.Entities.Product Delete(Domain.Entities.Product product)
    {
        _context.Products.Remove(product);
        return product;
    }

    public Domain.Entities.Product Update(Domain.Entities.Product product)
    {
        _context.Products.Update(product);
        return product;
    }

    public async Task<bool> NameExists(string name)
    {
        return await _context.Products.AnyAsync(product => product.Name == name);

    }

    public async Task<PromotionPrice> CreatePromotion(PromotionPrice promotionPrice)
    {
        await _context.Promotions.AddAsync(promotionPrice);

        return promotionPrice;
    }

    public async Task<bool> ExistsPromotionInDate(Guid productId, DateTime inicialDate, DateTime finalDate)
    {
        return await _context.Promotions.AnyAsync(pp =>
            pp.ProductId == productId &&
            inicialDate <= pp.FinalTime && finalDate >= pp.InitialTime);
    }

    public async Task<IList<PromotionPrice>> GetPromotionsToStartAsync(DateTime now, CancellationToken cancellationToken)
    {
        return await _context.Promotions
            .Include(p => p.Product)
            .Where(p => p.Active && p.InitialTime <= now && p.FinalTime >= now
                     && p.Product.CurrentPromotionalPrice == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<PromotionPrice>> GetPromotionsToFinishAsync(DateTime now, CancellationToken cancellationToken)
    {
        return await _context.Promotions
            .Include(p => p.Product)
            .Where(p => p.FinalTime < now && p.Product.CurrentPromotionalPrice != null)
            .ToListAsync(cancellationToken);
    }
}
