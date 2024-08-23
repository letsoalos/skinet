using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class DataContextSeed
{
    public static async Task SeedAsync(DataContext context)
    {
        if (!context.Products.Any())
        {
            var produtsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

            var products = JsonSerializer.Deserialize<List<Product>>(produtsData);

            if (products == null) return;

            context.Products.AddRange(products);

            await context.SaveChangesAsync();
        }
    }
}
