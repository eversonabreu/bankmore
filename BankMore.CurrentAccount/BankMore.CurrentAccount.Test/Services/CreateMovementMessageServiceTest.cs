using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using BankMore.CurrentAccount.Domain.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankMore.CurrentAccount.Test.Services;

public class CreateMovementMessageServiceTests
{
    private readonly Mock<IMovementRepository> _movementRepositoryMock;
    private readonly Mock<IIdempotenceService> _idempotenceServiceMock;
    private readonly Mock<ILogger<CreateMovementMessageService>> _loggerMock;
    private readonly CreateMovementMessageService _service;

    public CreateMovementMessageServiceTests()
    {
        _movementRepositoryMock = new Mock<IMovementRepository>();
        _idempotenceServiceMock = new Mock<IIdempotenceService>();
        _loggerMock = new Mock<ILogger<CreateMovementMessageService>>();

        _service = new CreateMovementMessageService(
            _movementRepositoryMock.Object,
            _idempotenceServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact(DisplayName = "Should return success payload when movement is successful")]
    public async Task HandleAsync_ShouldReturnSuccessPayload_WhenMovementIsSuccessful()
    {
        // Arrange
        var movementDto = new MovementDto
        (
            NumberAccount: 123456,
            Value: 150.00m,
            MovementType: MovementTypeEnum.Credit
        );

        var idempotenceKey = Guid.NewGuid().ToString();
        var payload = new IdempotenceRequestDto<MovementDto>
        (
            IdempotenceKey: idempotenceKey,
            PayloadRequisition: movementDto
        );

        _movementRepositoryMock
            .Setup(x => x.CreateMovementAsync(movementDto.NumberAccount, movementDto.MovementType, movementDto.Value))
            .ReturnsAsync(MovementOperationEnum.Success);

        _idempotenceServiceMock
            .Setup(x => x.SavePayloadResponseAsync(idempotenceKey, It.IsAny<object>()))
            .ReturnsAsync("success-payload");

        // Act
        var result = await _service.HandleAsync(payload);

        // Assert
        result.Should().Be("success-payload");

        _movementRepositoryMock.Verify(x => x.CreateMovementAsync(
            movementDto.NumberAccount, movementDto.MovementType, movementDto.Value), Times.Once);

        _idempotenceServiceMock.Verify(x => x.SavePayloadResponseAsync(idempotenceKey, It.IsAny<object>()), Times.Once);
    }

    [Fact(DisplayName = "Should log and return fallback payload when unexpected error occurs")]
    public async Task HandleAsync_ShouldReturnFallbackPayload_WhenUnhandledExceptionOccurs()
    {
        // Arrange
        var movementDto = new MovementDto
        (
            NumberAccount: 222222,
            Value: 50,
            MovementType: MovementTypeEnum.Debit
        );

        var idempotenceKey = Guid.NewGuid().ToString();

        var payload = new IdempotenceRequestDto<MovementDto>
        (
            IdempotenceKey: idempotenceKey,
            PayloadRequisition: movementDto
        );

        _movementRepositoryMock
            .Setup(x => x.CreateMovementAsync(movementDto.NumberAccount, movementDto.MovementType, movementDto.Value))
            .ReturnsAsync((MovementOperationEnum)999); // unexpected value

        _idempotenceServiceMock
            .Setup(x => x.SavePayloadResponseAsync(idempotenceKey, It.IsAny<object>()))
            .ReturnsAsync("fallback-payload");

        // Act
        var result = await _service.HandleAsync(payload);

        // Assert
        result.Should().Be("fallback-payload");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }
}