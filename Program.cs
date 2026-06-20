using MarketplaceOutsourcing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Marketplace Outsourcing API", Version = "v1" });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

try
{
    app.ApplyMigrationsAndSeed();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Could not connect to PostgreSQL or apply migrations.");
    throw;
}

app.Logger.LogInformation("Marketplace Outsourcing API is running.");
app.Run();
