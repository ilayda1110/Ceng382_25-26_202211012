using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Helpers;

public static class SeedData
{
    public static async Task Initialize(SchoolDbContext context, ILogger logger)
    {
        try
        {
            // Remove EnsureCreated() if using migrations
            // context.Database.EnsureCreated();

            // Seed Classes if empty
            if (!await context.Classes.AnyAsync())
            {
                var random = new Random();
                var classes = new List<Class>();

                for (int i = 1; i <= 100; i++)
                {
                    classes.Add(new Class
                    {
                        Name = $"Class {i}",
                        PersonCount = random.Next(20, 50),
                        Description = $"Description for Class {i}",
                        IsActive = true
                    });

                    // Save in batches of 20 to improve performance
                    if (i % 20 == 0)
                    {
                        await context.Classes.AddRangeAsync(classes);
                        await context.SaveChangesAsync();
                        classes.Clear();
                    }
                }

                // Add any remaining classes
                if (classes.Count > 0)
                {
                    await context.Classes.AddRangeAsync(classes);
                    await context.SaveChangesAsync();
                }

                logger.LogInformation("Seeded 100 classes");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding classes");
            }
    }
}
