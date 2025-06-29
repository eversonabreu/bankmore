using BankMore.Core.Infrastructure.Messaging;
using BankMore.Transfer.Domain.Dtos;
using BankMore.Transfer.Domain.Enums;
using BankMore.Transfer.Domain.Repositories;
using BankMore.Transfer.Domain.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankMore.Transfer.Test.Services;

public class TransferServiceTest
{
    private readonly Mock<ITransferRepository> _transferRepositoryMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<ILogger<TransferService>> _loggerMock;

    private readonly TransferService _service;

    public TransferServiceTest()
    {
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _messageServiceMock = new Mock<IMessageService>();
        _loggerMock = new Mock<ILogger<TransferService>>();

        // Setup the scope factory to return the mocked scope
        _scopeFactoryMock
            .Setup(x => x.CreateScope())
            .Returns(_serviceScopeMock.Object);

        // Setup service provider to return the scope factory when requested
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_scopeFactoryMock.Object);

        // Setup the service provider inside the scope to return IMessageService via GetService (not GetRequiredService)
        var nestedProviderMock = new Mock<IServiceProvider>();
        nestedProviderMock
            .Setup(x => x.GetService(typeof(IMessageService)))
            .Returns(_messageServiceMock.Object);

        _serviceScopeMock
            .Setup(x => x.ServiceProvider)
            .Returns(nestedProviderMock.Object);

        _service = new TransferService(
            _transferRepositoryMock.Object,
            _serviceProviderMock.Object,
            _loggerMock.Object
        );
    }

    [Fact(DisplayName = "Should return InvalidAccountDestination when origin and destination are the same")]
    public async Task CreateTransferAsync_ShouldReturnInvalidDestination_WhenAccountsAreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new TransferDto(
            CurrentAccountOriginId: id,
            CurrentAccountDestinationId: id,
            TransferValue: 100
        );

        // Act
        var result = await _service.CreateTransferAsync(dto);

        // Assert
        result.Should().Be(MovementOperationEnum.InvalidAccountDestination);
    }

    [Fact(DisplayName = "Should return InvalidValue when transfer amount is less than or equal to zero")]
    public async Task CreateTransferAsync_ShouldReturnInvalidValue_WhenValueIsInvalid()
    {
        // Arrange
        var dto = new TransferDto(
            CurrentAccountOriginId: Guid.NewGuid(),
            CurrentAccountDestinationId: Guid.NewGuid(),
            TransferValue: 0
        );

        // Act
        var result = await _service.CreateTransferAsync(dto);

        // Assert
        result.Should().Be(MovementOperationEnum.InvalidValue);
    }

    [Fact(DisplayName = "Should return Success and publish message when transfer is valid")]
    public async Task CreateTransferAsync_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var dto = new TransferDto(
            CurrentAccountOriginId: Guid.NewGuid(),
            CurrentAccountDestinationId: Guid.NewGuid(),
            TransferValue: 200
        );

        var entity = new Domain.Entities.Transfer
        {
            Id = Guid.NewGuid(),
            CurrentAccountOriginId = dto.CurrentAccountOriginId,
            CurrentAccountDestinationId = dto.CurrentAccountDestinationId,
            TransferValue = dto.TransferValue,
            TransferDate = DateTime.UtcNow
        };

        object publishedMessage = null;

        _transferRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Transfer>()))
            .ReturnsAsync(entity);

        _messageServiceMock
            .Setup(x => x.PublishAsync(Topics.CurrentAccountTransferTopicName, It.IsAny<object>()))
            .Callback<string, object>((_, message) =>
            {
                publishedMessage = message;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateTransferAsync(dto);

        // Assert
        result.Should().Be(MovementOperationEnum.Success);
        _transferRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);

        publishedMessage.Should().NotBeNull();

        var transferIdProp = publishedMessage.GetType().GetProperty("TransferId");
        var originIdProp = publishedMessage.GetType().GetProperty("OriginId");

        transferIdProp.Should().NotBeNull();
        originIdProp.Should().NotBeNull();

        transferIdProp.GetValue(publishedMessage).Should().Be(entity.Id);
        originIdProp.GetValue(publishedMessage).Should().Be(dto.CurrentAccountOriginId);
    }
}