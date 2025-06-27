using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Core.Web.Controlles;

[Authorize]
[ApiController]
public class ApplicationController : ControllerBase
{
    public long LoggedNumberAccount { get; set; }

    public string LoggedPersonName { get; set; }

    protected IActionResult CustomUnauthorized(string errorMessage, string errorCode)
        => Unauthorized(new { errorMessage, errorCode });

    protected IActionResult CustomBadRequest(string errorMessage, string errorCode)
        => BadRequest(new { errors = new List<dynamic> { new { message = errorMessage, code = errorCode } } });
}