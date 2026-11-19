using FinVault.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// SWAGGER - Required for Swagger UI
builder.Services.AddEndpointsApiExplorer(); // Discovers API endpoints
builder.Services.AddSwaggerGen();            // Registers Swagger generator

var app = builder.Build();

// SWAGGER - Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // Generates OpenAPI JSON
    app.UseSwaggerUI();  // Opens interactive Swagger UI
}

// HTTP Pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();