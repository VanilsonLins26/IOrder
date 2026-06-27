using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Category;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.infrastructure.Repositories.Category;

internal class CategoryRepository : ICategoryReadOnlyRepository, ICategoryWriteOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public CategoryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Domain.Entities.Category> Create(Domain.Entities.Category category)
    {
        await _dbContext.Categories.AddAsync(category);

        return category;
    }

    public Domain.Entities.Category Delete(Domain.Entities.Category category)
    {
        _dbContext.Categories.Remove(category);

        return category;
    }

    public async Task<IList<Domain.Entities.Category>> GetAll(Guid storeId)
    {
        return await _dbContext.Categories.AsNoTracking().Where(category => category.StoreId == storeId).ToListAsync();
    }

    public async Task<Domain.Entities.Category> GetByIdAsync(Guid id)
    {
        return await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(category => category.Id == id);
    }

    public async Task<Domain.Entities.Category> GetByIdTracking(Guid id)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == id);
    }

    public async Task<bool> NameExists(string name, Guid storeId)
    {
        return await _dbContext.Categories.Where(category => category.StoreId == storeId).AnyAsync(category => category.Name == name);

    }



}
