using BankMore.Core.Web.JWT;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Application.Responses;
using BankMore.CurrentAccount.Domain.Services.Contracts;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace BankMore.CurrentAccount.Application.Handlers;

public sealed class LoginCommandHandler(IAuthApplicationService authService,
    IConfiguration configuration) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginDto = await authService.LoginAsync(request.Code, request.Password);

        if (loginDto.LoginStatus == Domain.Enums.LoginStatusEnum.Success)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            string jwtToken = JwtToken.Create(jwtSettings, loginDto.NumberAccount.Value, loginDto.PersonName);

            return new LoginResponse(LoginStatus: Domain.Enums.LoginStatusEnum.Success,
                JwtToken: jwtToken, NumberAccount:  loginDto.NumberAccount, PersonName: loginDto.PersonName);
        }

        return new LoginResponse(LoginStatus: loginDto.LoginStatus, JwtToken: null, NumberAccount: null, PersonName: null);
    }
}