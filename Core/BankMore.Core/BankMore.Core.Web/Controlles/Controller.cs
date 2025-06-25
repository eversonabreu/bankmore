using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Core.Web.Controlles;

[Authorize]
public class Controller : ControllerBase
{
}