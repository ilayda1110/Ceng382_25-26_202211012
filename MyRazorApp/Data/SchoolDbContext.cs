/*
I created a Razor Page project. I have a login page and a Index page to show classes in a table. Right now, I store these class datas into json files when user presses "Export all in json" button. But i want to change it.
Your task: We will create a database in SSMS and use this database for our login and table 
operations. You need to implement all CRUD operations (Create, Read, Update, Delete).
All the JSON reads, writes, and updates in your website must be converted to database
operations for you to get a full score. 
(source code provided)



I already created a database named as "SchoolDb" and a query which includes:
CREATE TABLE Classes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    PersonCount INT NOT NULL,
    Description NVARCHAR(MAX),
    IsActive BIT NOT NULL
);

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL
);

Shall i change something here?



Gives error
MyRazorApp.Pages.IndexModel.OnGetAsync(Nullable<int> id) in Index.cshtml.cs
        var totalCount = await query.CountAsync();



Gives error as "SqlException: Invalid object name 'Classes'."



Okay now, what do i need to see in SSMS when i add a new class? How do i check if it saves data or not?



OMG IT IS WORKING I SAW THE NEWLY ADDED CLASS. But now i have two questions:
- I still have export json feature which works correctly. I dont if i should delete it or not tho
- There is no "Is Active" button in Index.cshtml so user cant choose it. Can we add that?



Okaay so i added the button but i have an issue. Right now, even though i choose "is active" it shows "No" in table BUT it saves correctly in database. It also saves "No" in json file. Can we fix that?
*/
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data;

public partial class SchoolDbContext : DbContext
{
    public SchoolDbContext()
    {
    }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    public DbSet<Class> Classes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=SchoolDbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
