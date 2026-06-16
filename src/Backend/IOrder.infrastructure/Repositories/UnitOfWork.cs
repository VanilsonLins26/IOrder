using IOrder.Domain.Repositories;
using IOrder.infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task Commit() => await _context.SaveChangesAsync();
}
