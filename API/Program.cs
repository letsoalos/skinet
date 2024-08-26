using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200","https://localhost:4200"));

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await DataContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
