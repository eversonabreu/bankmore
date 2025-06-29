using BankMore.Core.Web.Controlles;
using BankMore.Transfer.Domain.Dtos;
using BankMore.Transfer.Domain.Enums;
using BankMore.Transfer.Domain.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Transfer.API.Controllers;

[Route("api/transfer")]
public sealed class TransferController : ApplicationController
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateTransferAsync([FromBody] TransferDto transferDto,
        [FromServices] ITransferService transferService)
    {
        var response = await transferService.CreateTransferAsync(transferDto);

        if (response == MovementOperationEnum.Success)
            return Ok();

        return BadRequest(Enum.GetName(typeof(MovementOperationEnum), response));
    }
}