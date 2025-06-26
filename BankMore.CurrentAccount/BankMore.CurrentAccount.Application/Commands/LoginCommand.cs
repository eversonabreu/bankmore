using BankMore.CurrentAccount.Application.Responses;
using MediatR;

namespace BankMore.CurrentAccount.Application.Commands;

public sealed record LoginCommand(string Code, string Password) : IRequest<LoginResponse>;