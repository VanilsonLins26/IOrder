using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Events;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;

namespace UseCases.Test.Coverage;

public class DtoAndEntityCoverageTest
{
    [Fact]
    public void Cover_AssignCourierRequestDto()
    {
        var dto = new AssignCourierRequestDto();
        dto.CourierUserId = "user-123";
        dto.CourierUserId.ShouldBe("user-123");
    }

    [Fact]
    public void Cover_CreateReviewRequestDto()
    {
        var dto = new CreateReviewRequestDto();
        dto.StoreRating = 5;
        dto.CourierRating = 4;
        dto.Comment = "Great";
        
        dto.StoreRating.ShouldBe(5);
        dto.CourierRating.ShouldBe(4);
        dto.Comment.ShouldBe("Great");
    }

    [Fact]
    public void Cover_UpdateSaveCardRequestDto()
    {
        var dto = new UpdateSaveCardRequestDto();
        dto.SaveCard = true;
        dto.SaveCard.ShouldBeTrue();
    }

    [Fact]
    public void Cover_PublicKeyResponseDto()
    {
        var dto = new PublicKeyResponseDto { PublicKey = "pk_test_123" };
        dto.PublicKey.ShouldBe("pk_test_123");
    }

    [Fact]
    public void Cover_SelectedOptionResponseDto()
    {
        var dto = new SelectedOptionResponseDto { OptionId = Guid.NewGuid(), OptionName = "Test", PriceModifier = 1.5m };
        dto.OptionId.ShouldNotBe(Guid.Empty);
        dto.OptionName.ShouldBe("Test");
        dto.PriceModifier.ShouldBe(1.5m);
    }

    [Fact]
    public void Cover_SelectedOption()
    {
        var entity = new SelectedOption { OptionId = Guid.NewGuid(), OptionName = "Test", PriceModifier = 1.5m };
        entity.OptionId.ShouldNotBe(Guid.Empty);
        entity.OptionName.ShouldBe("Test");
        entity.PriceModifier.ShouldBe(1.5m);
    }

    [Fact]
    public void Cover_UserCard()
    {
        var card = new IOrder.Domain.Entities.UserCard { UserId = "123", GatewayCardId = "pm_123", LastFourDigits = "4242", Brand = "visa" };
        card.UserId.ShouldBe("123");
        card.GatewayCardId.ShouldBe("pm_123");
        card.LastFourDigits.ShouldBe("4242");
        card.Brand.ShouldBe("visa");
    }

    [Fact]
    public void Cover_Events()
    {
        var abandoned = new CartAbandonedEvent("user123", "9999");
        abandoned.UserId.ShouldBe("user123");

        var reviewed = new OrderReviewedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5);
        reviewed.OrderId.ShouldNotBe(Guid.Empty);

        var promoActivated = new PromotionActivatedEvent(Guid.NewGuid(), Guid.NewGuid(), "Prod", 10m);
        promoActivated.ProductId.ShouldNotBe(Guid.Empty);

        var promoDeactivated = new PromotionDeactivatedEvent(Guid.NewGuid(), Guid.NewGuid(), "Prod");
        promoDeactivated.ProductId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Cover_UnauthorizedException()
    {
        var ex = new UnauthorizedException("Not allowed");
        ex.Message.ShouldBe("");
        ex.GetErrorMessages().ShouldContain("Not allowed");
    }
}
