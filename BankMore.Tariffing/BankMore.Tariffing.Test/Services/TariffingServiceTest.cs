using BankMore.Tariffing.Domain.Repositories;
using BankMore.Tariffing.Domain.Services.Implementations;
using BankMore.Tariffing.Domain.Constants;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using BankMore.Core.Infrastructure.Messaging;

namespace BankMore.Tariffing.Test.Services;

public class TariffingServiceTest
{
    private readonly Mock<ITariffingRepository> _tariffingRepositoryMock;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<ILogger<TariffingService>> _loggerMock;
    private readonly TariffingService _service;

    public TariffingServiceTest()
    {
        _tariffingRepositoryMock = new Mock<ITariffingRepository>();
        _messageServiceMock = new Mock<IMessageService>();
        _loggerMock = new Mock<ILogger<TariffingService>>();

        _service = new TariffingService(
            _tariffingRepositoryMock.Object,
            _messageServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact(DisplayName = "Should create tariffing and publish message when no existing tariffing found")]
    public async Task CreateTariffingAsync_ShouldCreateAndPublish_WhenNoExistingTariffing()
    {
        // Arrange
        var transferId = Guid.NewGuid();
        var currentAccountOriginId = Guid.NewGuid();

        _tariffingRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Tariffing, bool>>>()))
            .ReturnsAsync((Domain.Entities.Tariffing)null);

        _tariffingRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Tariffing>()))
            .ReturnsAsync(new Domain.Entities.Tariffing
            {
                TransferId = transferId,
                RateValue = Constants.RateValue,
                DateTransaction = DateTime.UtcNow
            });


        _tariffingRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        object publishedMessage = null;

        _messageServiceMock
            .Setup(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((_, message) =>
            {
                publishedMessage = message;
            })
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateTariffingAsync(transferId, currentAccountOriginId);

        // Assert
        _tariffingRepositoryMock.Verify(x => x.CreateAsync(It.Is<Domain.Entities.Tariffing>(t =>
            t.TransferId == transferId &&
            t.RateValue == Constants.RateValue
        )), Times.Once);

        _tariffingRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);

        publishedMessage.Should().NotBeNull();

        var idProp = publishedMessage.GetType().GetProperty("Id");
        var rateValueProp = publishedMessage.GetType().GetProperty("Value");

        idProp.GetValue(publishedMessage).Should().Be(currentAccountOriginId);
        rateValueProp.GetValue(publishedMessage).Should().Be(Constants.RateValue);
    }

    [Fact(DisplayName = "Should not create tariffing if already exists but still publish message")]
    public async Task CreateTariffingAsync_ShouldNotCreateTariffing_WhenExistingFound_ButPublishMessage()
    {
        // Arrange
        var transferId = Guid.NewGuid();
        var currentAccountOriginId = Guid.NewGuid();

        _tariffingRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Tariffing, bool>>>()))
            .ReturnsAsync(new Domain.Entities.Tariffing { TransferId = transferId });

        object publishedMessage = null;

        _messageServiceMock
            .Setup(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((_, message) =>
            {
                publishedMessage = message;
            })
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateTariffingAsync(transferId, currentAccountOriginId);

        // Assert
        _tariffingRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Domain.Entities.Tariffing>()), Times.Never);
        _tariffingRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);

        publishedMessage.Should().NotBeNull();

        var idProp = publishedMessage.GetType().GetProperty("Id");
        var rateValueProp = publishedMessage.GetType().GetProperty("Value");

        idProp.GetValue(publishedMessage).Should().Be(currentAccountOriginId);
        rateValueProp.GetValue(publishedMessage).Should().Be(Constants.RateValue);
    }

    [Fact(DisplayName = "Should log error and rethrow when exception occurs")]
    public async Task CreateTariffingAsync_ShouldLogErrorAndThrow_WhenExceptionOccurs()
    {
        // Arrange
        var transferId = Guid.NewGuid();
        var currentAccountOriginId = Guid.NewGuid();

        _tariffingRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Tariffing, bool>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = () => _service.CreateTariffingAsync(transferId, currentAccountOriginId);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error when tariffing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "GetTariffingsAsync should return tariffings from repository")]
    public async Task GetTariffingsAsync_ShouldReturnTariffings()
    {
        // Arrange
        var tariffings = new List<Domain.Entities.Tariffing>
        {
            new() { TransferId = Guid.NewGuid(), RateValue = Constants.RateValue, DateTransaction = DateTime.UtcNow },
            new() { TransferId = Guid.NewGuid(), RateValue = Constants.RateValue, DateTransaction = DateTime.UtcNow }
        };

        _tariffingRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Tariffing, bool>>>()))
            .ReturnsAsync(tariffings);

        // Act
        var result = await _service.GetTariffingsAsync();

        // Assert
        result.Should().BeEquivalentTo(tariffings);
    }
}