
using FinVault.API.Extensions;
using FinVault.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURE LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 2. REGISTER CONTROLLERS AND VALIDATION FILTER
builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ValidationFilter>();
});

// 3. DEPENDENCY INJECTION
builder.Services.AddInfrastructure(builder.Configuration);

// 4. SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. GLOBAL EXCEPTION HANDLING
app.UseGlobalExceptionMiddleware();

// 6. SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7. HTTP PIPELINE
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
