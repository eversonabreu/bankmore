using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swg =>
{
    swg.SwaggerDoc("v1", new OpenApiInfo { Title = "BankMore.CurrentAccount.API", Version = "v1" });

    swg.AddSecurityDefinition("authorization", new OpenApiSecurityScheme
    {
        Name = "authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header
    });

    swg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "authorization"
              }
             },
             Array.Empty<string>()
       }
    });
});

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMvc();
app.Run();