using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System.Reflection;

namespace UseCases.Test.Delivery;

public class RejectDeliveryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var assignment = DeliveryAssignmentBuilder.Build();
        // assignment is Pending initially

        var useCase = CreateUseCase(assignment.CourierUserId, assignment);

        var response = await useCase.Execute(assignment.Id);

        response.ShouldNotBeNull();
        response.Status.ShouldBe(IOrder.Communication.Enums.AssignmentStatusDto.Rejected);
    }

    [Fact]
    public async Task Error_AssignmentNotFound()
    {
        var useCase = CreateUseCase("courier-1", null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Atribuição não encontrada.");
    }

    [Fact]
    public async Task Error_CourierNotMatch()
    {
        var assignment = DeliveryAssignmentBuilder.Build();
        assignment.CourierUserId = "courier-1";
        
        var useCase = CreateUseCase("courier-2", assignment);

        Func<Task> act = async () => await useCase.Execute(assignment.Id);

        var exception = await act.ShouldThrowAsync<UnauthorizedStoreException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Você não tem permissão para rejeitar esta atribuição.");
    }

    [Fact]
    public async Task Error_StatusNotPending()
    {
        var assignment = DeliveryAssignmentBuilder.Build();
        assignment.Accept();
        
        var useCase = CreateUseCase(assignment.CourierUserId, assignment);

        Func<Task> act = async () => await useCase.Execute(assignment.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Esta atribuição não está pendente.");
    }

    private RejectDeliveryUseCase CreateUseCase(
        string loggedUserId,
        IOrder.Domain.Entities.DeliveryAssignment? assignment)
    {
        var loggedUser = LoggedUserBuilder.Build(loggedUserId);
        
        var assignmentReadOnly = new DeliveryAssignmentReadOnlyRepositoryBuilder();
        var assignmentWriteOnly = new DeliveryAssignmentWriteOnlyRepositoryBuilder();
        if (assignment is not null)
        {
            assignmentWriteOnly.GetByIdTrackingAsync(assignment);
        }

        var uow = UnitOfWorkBuilder.Build();

        return new RejectDeliveryUseCase(
            loggedUser,
            assignmentReadOnly.Build(),
            assignmentWriteOnly.Build(),
            uow);
    }
}
