using BankMore.Core.Web.Controlles;
using BankMore.Tariffing.Domain.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Tariffing.API.Controllers;

[Route("api/tariffing")]
public sealed class TariffingController : ApplicationController
{
    // Este endpoint é bem simples mesmo, usei apenas para testes
    // É claro que poderia ter filtros, paginação...
    // A comunicação de tarificação com a API de conta-corrente é feita via mensagem Kafka

    [HttpGet("all-tariffings")]
    public async Task<IActionResult> GetTariffingsAsync([FromServices] ITariffingService tariffingService)
        => Ok(await tariffingService.GetTariffingsAsync());
}