using UnitConversion.Api.Interfaces;
using UnitConversion.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services in Dependency Injection (DI) container
builder.Services.AddSingleton<IUnitCatalogService, UnitCatalogService>();
builder.Services.AddScoped<IUnitConversionService, UnitConversionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
