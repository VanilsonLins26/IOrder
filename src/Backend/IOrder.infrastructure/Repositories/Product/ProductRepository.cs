using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Pagination;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.SeedWork.Pagination;
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



    public async Task<PagedList<Domain.Entities.Product>> GetAllPagFiltroPrecoAsync(ProductSearchQuery productFilter)
    {
        var query = _context.Products.AsNoTracking();

        if (productFilter.Price.HasValue && productFilter.PriceFilter.HasValue)
        { 
            query = productFilter.PriceFilter.Value switch
            {
                PriceFilterType.GreaterThan => query.Where(p => (p.CurrentPromotionalPrice ?? p.Price) > productFilter.Price),

                PriceFilterType.LessThan => query.Where(p => (p.CurrentPromotionalPrice ?? p.Price) < productFilter.Price),

                PriceFilterType.EqualTo => query.Where(p => (p.CurrentPromotionalPrice ?? p.Price) == productFilter.Price),

                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(productFilter.Name))
            query = query.Where(p => p.Name.Contains(productFilter.Name));

        if (productFilter.StoreId.HasValue)
            query = query.Where(p => p.StoreId == productFilter.StoreId.Value);

        var property = productFilter.OrderBy?.ToLower().Trim();

        query = property switch
        {
            "name" => productFilter.IsDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            "price" => productFilter.IsDescending
                ? query.OrderByDescending(p => p.CurrentPromotionalPrice ?? p.Price)
                : query.OrderBy(p => p.CurrentPromotionalPrice ?? p.Price),

            _ => productFilter.IsDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
        };

        return await query.ToPagedListAsync(productFilter.PageNumber, productFilter.PageSize);
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
}
