namespace IOrder.Domain.SeedWork.Pagination;

public class StoreSearchQuery : PaginationQuery
{
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }
}
