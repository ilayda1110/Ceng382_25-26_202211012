using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Helpers;

public static class SeedData
{
    public static void Initialize(SchoolDbContext context)
    {
        try
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            if (!context.Classes.Any())
            {
                var random = new Random();
                for (int i = 1; i <= 100; i++)
                {
                    context.Classes.Add(new Class
                    {
                        Name = $"Class {i}",
                        PersonCount = random.Next(20, 50),
                        Description = $"Description for Class {i}",
                        IsActive = true
                    });
                }
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
}