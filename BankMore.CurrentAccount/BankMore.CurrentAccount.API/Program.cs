using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Infrastructure.Database;
using BankMore.Core.Web.JWT;
using BankMore.Core.Web.Swagger;
using Microsoft.EntityFrameworkCore;
using BankMore.CurrentAccount.Domain.Helpers;
using BankMore.CurrentAccount.Infrastructure;
using BankMore.CurrentAccount.Application;
using BankMore.CurrentAccount.Domain;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration("BankMore.CurrentAccount.API", "v1");
builder.Services.ConfigureJwtServices(builder.Configuration);
builder.Services.AddSQliteConfiguredDbContext<ApplicationDbContext>(Constants.ApplicationDatabaseName);
builder.Services.AddInfrastructureServices();
builder.Services.AddDomainServices();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMvc();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();