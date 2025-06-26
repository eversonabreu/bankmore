using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Core.Web.Controlles;

[Authorize]
[ApiController]
public class ApplicationController : ControllerBase
{
    public long LoggedNumberAccount { get; set; }

    public string LoggedPersonName { get; set; }

    protected IActionResult CustomUnauthorized(string messageErrorCode, string messageErrorDescription = null)
        => Unauthorized(new { messageErrorCode, messageErrorDescription });
}