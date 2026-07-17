using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Category;
using IOrder.Exceptions.ExceptionBase;
using System.Linq;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Category.Commands;

public class UpdateCategoryPositionsUseCase : IUpdateCategoryPositionsUseCase
{
    private readonly ICategoryWriteOnlyRepository _writeOnlyRepository;
    private readonly IStorePermissionService _permissionService;
    private readonly IUnitOfWork _uof;

    public UpdateCategoryPositionsUseCase(
        ICategoryWriteOnlyRepository writeOnlyRepository,
        IStorePermissionService permissionService,
        IUnitOfWork uof)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _permissionService = permissionService;
        _uof = uof;
    }

    public async Task Execute(UpdateCategoryPositionsRequestDto request)
    {
        if (request.Positions == null || !request.Positions.Any())
            return;

        var loggedStoreId = await _permissionService.GetLoggedUserStoreIdAsync();
        
        foreach (var pos in request.Positions)
        {
            var category = await _writeOnlyRepository.GetByIdTracking(pos.CategoryId);
            
            if (category != null && category.StoreId == loggedStoreId)
            {
                category.Position = pos.Position;
            }
        }

        await _uof.Commit();
    }
}

