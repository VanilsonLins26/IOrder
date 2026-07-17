using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;
using Moq;

namespace CommomTestUtilities.Repositories;

public class ChatReadOnlyRepositoryBuilder
{
    private readonly Mock<IChatReadOnlyRepository> _mock;

    public ChatReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IChatReadOnlyRepository>();
    }

    public ChatReadOnlyRepositoryBuilder GetConversationsAsync(IList<ConversationSummary> conversations)
    {
        _mock.Setup(r => r.GetConversationsAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(conversations);
        return this;
    }

    public ChatReadOnlyRepositoryBuilder GetConversationsCountAsync(int count)
    {
        _mock.Setup(r => r.GetConversationsCountAsync(It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync(count);
        return this;
    }

    public ChatReadOnlyRepositoryBuilder GetMessagesAsync(IList<OrderMessage> messages)
    {
        _mock.Setup(r => r.GetMessagesAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(messages);
        return this;
    }

    public ChatReadOnlyRepositoryBuilder GetMessagesCountAsync(int count)
    {
        _mock.Setup(r => r.GetMessagesCountAsync(It.IsAny<Guid>())).ReturnsAsync(count);
        return this;
    }

    public ChatReadOnlyRepositoryBuilder GetUnreadCountAsync(int count)
    {
        _mock.Setup(r => r.GetUnreadCountAsync(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(count);
        return this;
    }

    public IChatReadOnlyRepository Build() => _mock.Object;
}
