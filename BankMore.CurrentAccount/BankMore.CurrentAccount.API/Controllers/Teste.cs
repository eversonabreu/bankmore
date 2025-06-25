using Microsoft.AspNetCore.Mvc;

namespace BankMore.CurrentAccount.API.Controllers;

[Route("api/teste")]
public sealed class Teste : ControllerBase
{
    [HttpGet("xpto")]
    public IActionResult GetTest()
    {
        return Ok(DateTime.Now);
    }
}
