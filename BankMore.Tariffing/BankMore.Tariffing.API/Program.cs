using BankMore.Core.Web.JWT;
using BankMore.Core.Web.Swagger;
using BankMore.Core.Web;
using BankMore.Tariffing.Infrastructure.Database;
using BankMore.Tariffing.Domain.Constants;
using BankMore.Core.Infrastructure.Database;
using BankMore.Core.Infrastructure.Messaging;
using BankMore.Tariffing.Infrastructure;
using BankMore.Tariffing.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration("BankMore.Tariffing.API", "v1");
builder.Services.ConfigureJwtServices(builder.Configuration);
builder.Services.AddCoreServices();
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