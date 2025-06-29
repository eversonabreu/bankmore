using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using BankMore.Core.Infrastructure.Messaging;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Entities;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.CurrentAccount.Domain.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BankMore.CurrentAccount.Tests.Services;

public class IdempotenceServiceTests
{
    private readonly Mock<IIdempotenceRepository> _repositoryMock;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<IdempotenceService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<IMessageTopicHandler> _messageTopicHandlerMock;

    private readonly IdempotenceService _service;

    public IdempotenceServiceTests()
    {
        _repositoryMock = new Mock<IIdempotenceRepository>();
        _loggerMock = new Mock<ILogger<IdempotenceService>>();
        _configurationMock = new Mock<IConfiguration>();
        _messageServiceMock = new Mock<IMessageService>();
        _messageTopicHandlerMock = new Mock<IMessageTopicHandler>();

        // Real memory cache (can be replaced by mock if needed)
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _service = new IdempotenceService(
            _repositoryMock.Object,
            _memoryCache,
            _loggerMock.Object,
            _configurationMock.Object,
            _messageServiceMock.Object
        );
    }

    [Fact(DisplayName = "Should return IdempotenceRequestBodyMismatch when payloads differ")]
    public async Task ProcessAsync_ShouldReturnMismatch_WhenPayloadsAreDifferent()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var originalPayload = new MovementDto(1234, MovementTypeEnum.Credit, 100);
        var cachedPayload = JsonSerializer.Serialize(new MovementDto(1234, MovementTypeEnum.Debit, 999));

        _repositoryMock
            .Setup(x => x.SingleOrDefaultAsync(x => x.Key == key))
            .ReturnsAsync(new Idempotence
            {
                Key = key,
                PayloadRequisition = cachedPayload,
                PayloadResponse = null
            });

        // Act
        var (status, response) = await _service.ProcessAsync(
            key,
            originalPayload,
            "topic-name",
            _messageTopicHandlerMock.Object
        );

        // Assert
        status.Should().Be(MovementOperationEnum.IdempotenceRequestBodyMismatch);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return Success when cached payload and response exist")]
    public async Task ProcessAsync_ShouldReturnSuccess_WhenCachedWithResponse()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var payload = new MovementDto(1234, MovementTypeEnum.Credit, 100);
        var serialized = JsonSerializer.Serialize(payload);

        _repositoryMock
            .Setup(x => x.SingleOrDefaultAsync(x => x.Key == key))
            .ReturnsAsync(new Idempotence
            {
                Key = key,
                PayloadRequisition = serialized,
                PayloadResponse = "cached-response"
            });

        // Act
        var (status, response) = await _service.ProcessAsync(
            key,
            payload,
            "topic-name",
            _messageTopicHandlerMock.Object
        );

        // Assert
        status.Should().Be(MovementOperationEnum.Success);
        response.Should().Be("cached-response");
    }

    [Fact(DisplayName = "Should return WaitingFinishProccess when no response is yet available")]
    public async Task ProcessAsync_ShouldReturnWaiting_WhenNoResponseYet()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var payload = new MovementDto(1234, MovementTypeEnum.Credit, 100);
        var serialized = JsonSerializer.Serialize(payload);

        _repositoryMock
            .Setup(x => x.SingleOrDefaultAsync(x => x.Key == key))
            .ReturnsAsync(new Idempotence
            {
                Key = key,
                PayloadRequisition = serialized,
                PayloadResponse = null
            });

        // Act
        var (status, response) = await _service.ProcessAsync(
            key,
            payload,
            "topic-name",
            _messageTopicHandlerMock.Object
        );

        // Assert
        status.Should().Be(MovementOperationEnum.WaitingFinishProccess);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return FatalErrorProccessing when idempotence cannot be created")]
    public async Task ProcessAsync_ShouldReturnFatalError_WhenCreationFails()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var payload = new MovementDto(1234, MovementTypeEnum.Credit, 100);

        _repositoryMock
            .Setup(x => x.SingleOrDefaultAsync(x => x.Key == key))
            .ReturnsAsync((Idempotence)null);

        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Idempotence>()))
            .ThrowsAsync(new Exception("db-error"));

        // Act
        var (status, response) = await _service.ProcessAsync(
            key,
            payload,
            "topic-name",
            _messageTopicHandlerMock.Object
        );

        // Assert
        status.Should().Be(MovementOperationEnum.FatalErrorProccessing);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return Success when HandleAsync returns result")]
    public async Task ProcessAsync_ShouldReturnSuccess_WhenHandledSuccessfully()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var payload = new MovementDto(1234, MovementTypeEnum.Credit, 100);

        _repositoryMock
            .Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Idempotence, bool>>>()))
            .ReturnsAsync((Idempotence)null);

        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Idempotence>()));
        _repositoryMock
            .Setup(x => x.SaveChangesAsync());

        _messageTopicHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<IdempotenceRequestDto<MovementDto>>()))
            .ReturnsAsync("handler-response");

        // Act
        var (status, response) = await _service.ProcessAsync(
            key,
            payload,
            "topic-name",
            _messageTopicHandlerMock.Object
        );

        // Assert
        status.Should().Be(MovementOperationEnum.Success);
        response.Should().Be("handler-response");
    }

    [Fact(DisplayName = "Should fallback to Kafka when HandleAsync fails")]
    public async Task ProcessAsync_ShouldReturnWaiting_WhenHandlerThrowsAndMessageIsPublished()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var payload = new MovementDto(1234, MovementTypeEnum.Credit, 100);

        _repositoryMock
            .Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<Idempotence, bool>>>()))
            .ReturnsAsync((Idempotence)null);

        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Idempotence>()));
        _repositoryMock.Setup(x => x.SaveChangesAsync());

        _messageTopicHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<IdempotenceRequestDto<MovementDto>>()))
            .ThrowsAsync(new Exception("handler error"));

        _messageServiceMock
            .Setup(x => x.PublishAsync("topic-name", It.IsAny<IdempotenceRequestDto<MovementDto>>()))
            .Returns(Task.CompletedTask); // simulate success

        // Act
        var (status, response) = await _service.ProcessAsync(
            key,
            payload,
            "topic-name",
            _messageTopicHandlerMock.Object
        );

        // Assert
        status.Should().Be(MovementOperationEnum.WaitingFinishProccess);
        response.Should().BeNull();
    }
}