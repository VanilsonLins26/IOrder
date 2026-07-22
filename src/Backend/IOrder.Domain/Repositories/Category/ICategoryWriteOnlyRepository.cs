using IOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Category;

public interface ICategoryWriteOnlyRepository
{
    Task<Entities.Category> Create(Entities.Category category);

    Entities.Category Delete(Entities.Category category);

    Task<Entities.Category> GetByIdTracking(Guid id);

    void Update(Entities.Category category);


}
