using BankMore.Core.Web.JWT;
using BankMore.Core.Web.Swagger;
using BankMore.Transfer.Infrastructure.Database;
using BankMore.Core.Infrastructure.Database;
using BankMore.Transfer.Domain.Constants;
using BankMore.Core.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using BankMore.Transfer.Infrastructure;
using BankMore.Transfer.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration("BankMore.Transfer.API", "v1");
builder.Services.ConfigureJwtServices(builder.Configuration);
builder.Services.AddSQliteConfiguredDbContext<ApplicationDbContext>(Constants.ApplicationDatabaseName);
builder.Services.AddInfrastructureServices();
builder.Services.AddDomainServices();
builder.Services.AddSingleton<IMessageService, MessageService>();

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