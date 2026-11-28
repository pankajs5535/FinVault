
using FinVault.API.Extensions;
using FinVault.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURE LOGGING

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 2. REGISTER CONTROLLERS AND VALIDATION FILTER

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ValidationFilter>();
});

// 3. DEPENDENCY INJECTION

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// 4. GLOBAL EXCEPTION HANDLING

app.UseGlobalExceptionMiddleware();

// 5. SWAGGER

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. HTTP PIPELINE

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
