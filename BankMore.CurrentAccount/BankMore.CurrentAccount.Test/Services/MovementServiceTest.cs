using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.Core.Infrastructure.Messaging;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using BankMore.CurrentAccount.Domain.Services.Implementations;

namespace BankMore.CurrentAccount.Test.Services;

public class MovementServiceTests
{
    private readonly Mock<IIdempotenceService> _idempotenceServiceMock;
    private readonly Mock<IMessageTopicHandler> _messageTopicHandlerMock;
    private readonly Mock<ICurrentAccountRepository> _currentAccountRepositoryMock;
    private readonly Mock<IMovementRepository> _movementRepositoryMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IConfiguration> _configurationMock;

    private readonly MovementService _service;

    public MovementServiceTests()
    {
        _idempotenceServiceMock = new Mock<IIdempotenceService>();
        _messageTopicHandlerMock = new Mock<IMessageTopicHandler>();
        _currentAccountRepositoryMock = new Mock<ICurrentAccountRepository>();
        _movementRepositoryMock = new Mock<IMovementRepository>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _configurationMock = new Mock<IConfiguration>();

        _service = new MovementService(
            _idempotenceServiceMock.Object,
            _messageTopicHandlerMock.Object,
            _currentAccountRepositoryMock.Object,
            _movementRepositoryMock.Object,
            _httpClientFactoryMock.Object,
            _configurationMock.Object
        );
    }

    [Fact(DisplayName = "Should return IdempotenceKeyNullOrEmpty when key is null or whitespace")]
    public async Task GetOrSaveMovementAsync_ShouldReturnKeyNull_WhenKeyIsEmpty()
    {
        // Arrange
        var idempotenceKey = "   ";

        // Act
        var (status, response) = await _service.GetOrSaveMovementAsync(
            idempotenceKey,
            1234,
            MovementTypeEnum.Debit,
            100,
            1234
        );

        // Assert
        status.Should().Be(MovementOperationEnum.IdempotenceKeyNullOrEmpty);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return InvalidType when request account is not the logged one on debit")]
    public async Task GetOrSaveMovementAsync_ShouldReturnInvalidType_WhenAccountMismatchOnDebit()
    {
        // Arrange
        var idempotenceKey = Guid.NewGuid().ToString();
        long requestAccount = 2000;
        long loggedAccount = 1000;

        // Act
        var (status, response) = await _service.GetOrSaveMovementAsync(
            idempotenceKey,
            requestAccount,
            MovementTypeEnum.Debit,
            100,
            loggedAccount
        );

        // Assert
        status.Should().Be(MovementOperationEnum.InvalidType);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return error from current account repository if any")]
    public async Task GetOrSaveMovementAsync_ShouldReturnRepositoryError_WhenAccountHasIssue()
    {
        // Arrange
        var idempotenceKey = Guid.NewGuid().ToString();
        long account = 1234;

        _currentAccountRepositoryMock
            .Setup(x => x.GetCurrentAccountByNumberAsync(account))
            .ReturnsAsync((new(), MovementOperationEnum.InvalidAccount));

        // Act
        var (status, response) = await _service.GetOrSaveMovementAsync(
            idempotenceKey,
            account,
            MovementTypeEnum.Credit,
            100,
            account
        );

        // Assert
        status.Should().Be(MovementOperationEnum.InvalidAccount);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return FatalErrorProccessing when ProcessAsync returns fatal error in response")]
    public async Task GetOrSaveMovementAsync_ShouldReturnFatalError_WhenPayloadHasFatalMessage()
    {
        // Arrange
        var idempotenceKey = Guid.NewGuid().ToString();
        long account = 1234;

        _currentAccountRepositoryMock
            .Setup(x => x.GetCurrentAccountByNumberAsync(account))
            .ReturnsAsync((new(), null));

        _idempotenceServiceMock
            .Setup(x => x.ProcessAsync(
                idempotenceKey,
                It.IsAny<MovementDto>(),
                Topics.CurrentAccountMovementTopicName,
                _messageTopicHandlerMock.Object))
            .ReturnsAsync((
                MovementOperationEnum.Success,
                Constants.ApplicationMessages.FatalErrorMovementCurrentAccountError + " | stack trace"));

        // Act
        var (status, response) = await _service.GetOrSaveMovementAsync(
            idempotenceKey,
            account,
            MovementTypeEnum.Credit,
            500,
            account
        );

        // Assert
        status.Should().Be(MovementOperationEnum.FatalErrorProccessing);
        response.Should().BeNull();
    }

    [Fact(DisplayName = "Should return successful operation and payload from ProcessAsync")]
    public async Task GetOrSaveMovementAsync_ShouldReturnSuccess_WhenProcessSucceeds()
    {
        // Arrange
        var idempotenceKey = Guid.NewGuid().ToString();
        long account = 9876;

        _currentAccountRepositoryMock
            .Setup(x => x.GetCurrentAccountByNumberAsync(account))
            .ReturnsAsync((new(), null));

        _idempotenceServiceMock
            .Setup(x => x.ProcessAsync(
                idempotenceKey,
                It.IsAny<MovementDto>(),
                Topics.CurrentAccountMovementTopicName,
                _messageTopicHandlerMock.Object))
            .ReturnsAsync((MovementOperationEnum.Success, "valid-response-payload"));

        // Act
        var (status, response) = await _service.GetOrSaveMovementAsync(
            idempotenceKey,
            null, // simulate not sending in body
            MovementTypeEnum.Credit,
            1200,
            account // fallback to logged account
        );

        // Assert
        status.Should().Be(MovementOperationEnum.Success);
        response.Should().Be("valid-response-payload");
    }
}