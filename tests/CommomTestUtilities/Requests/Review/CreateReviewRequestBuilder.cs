using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Review;

public class CreateReviewRequestBuilder
{
    public static CreateReviewRequestDto Build(bool withCourierRating = false)
    {
        return new Faker<CreateReviewRequestDto>("pt_BR")
            .RuleFor(r => r.StoreRating, f => f.Random.Int(1, 5))
            .RuleFor(r => r.CourierRating, f => withCourierRating ? f.Random.Int(1, 5) : null)
            .RuleFor(r => r.Comment, f => f.Lorem.Sentence())
            .Generate();
    }
}
