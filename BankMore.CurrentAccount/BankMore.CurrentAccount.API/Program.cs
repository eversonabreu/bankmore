using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Infrastructure.Database;
using BankMore.Core.Web.JWT;
using BankMore.Core.Web.Swagger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration("BankMore.CurrentAccount.API", "v1");
builder.Services.ConfigureJwtServices(builder.Configuration);
builder.Services.AddSQliteConfiguredDbContext<ApplicationDbContext>("CurrentAccount.db");

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMvc();
app.Run();