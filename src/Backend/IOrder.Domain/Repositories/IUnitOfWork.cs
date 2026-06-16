namespace IOrder.Domain.Repositories;

public interface IUnitOfWork
{
    public Task Commit();
}
