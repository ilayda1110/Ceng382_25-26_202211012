/*
I want you to detect the problem about redirecting to pages. When i start my program, the link looks like "/Login?ReturnUrl=%2F" even though it should just show /Login. I tried to debug my code in Index.cshtml.cs but it didnt even get into first line of OnGetAsync(). You can try to fix this problem by keeping this current structure (when we start the program, it starts with table page which is Index even though it should start with Login page) or totally change the codes inside of files. My teacher adviced me to change it since it should work like that
(source code provided)

For step 1, it shows error as 'PageActionEndpointConventionBuilder' does not contain a definition for 'AddRazorPagesOptions' and the best extension method overload 'MvcRazorPagesMvcBuilderExtensions.AddRazorPagesOptions(IMvcBuilder, Action<RazorPagesOptions>)' requires a receiver of type 'Microsoft.Extensions.DependencyInjection.IMvcBuilder'

When i run the program, it gives error as "AmbiguousMatchException: The request matched multiple endpoints. Matches:

/Index
/Login"
Dont you think that we should swap these codes to make it run Index as login page and then Dashboard to show table rather than forcing program to start with Login? I think it will be more safe to do that.

When i enter as admin it gives error as "Şu web adresi için web sayfası bulunamadı:http://localhost:5254/Login?ReturnUrl=%2FDashboard"

It still gives the same error. I will provide my code files, can you check and correct them if you see a problem?
(source code provided)

Okay everything works for now. The next thing we need to do is adding sentetic data. I need to create 100 data and add to database at start (it shouldnt add 100 more when you restart the program. it should be a one time thing)

It didnt generate data. Let me send you my SchoolDbContext.cs:
(source code provided)
Also dont forget to display sentetic data in table since they are all active

This was my query btw:
SELECT TOP (1000) [Id]
      ,[Name]
      ,[PersonCount]
      ,[Description]
      ,[IsActive]
  FROM [SchoolDb].[dbo].[Classes]
Also it gave error as "SqlException: Invalid object name 'Classes'.
Microsoft.Data.SqlClient.SqlCommand+<>c.<ExecuteDbDataReaderAsync>b__211_0(Task<SqlDataReader> result)"

Gave error as "Add-Migration : The term 'Add-Migration' is not recognized as the name of a cmdlet, function, script file, or operable program. Check the spell
ing of the name, or if a path was included, verify that the path is correct and try again.
At line:1 char:1
+ Add-Migration InitialCreate
+ ~~~~~~~~~~~~~
    + CategoryInfo          : ObjectNotFound: (Add-Migration:String) [], CommandNotFoundException
    + FullyQualifiedErrorId : CommandNotFoundException"

Okay it created the sentetic data and i can see it on database but i have another issue. When i press page 2 for example, it directs me to login page.
(source code provided)

Still doesnt work and the url looks like "/?PageNumber=2". Is that because of null reference warnings or something like that?
(source code provided)
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Helpers;
using MyRazorApp.Models;
using MyRazorApp.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace MyRazorApp.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly SchoolDbContext _context;

    public DashboardModel(SchoolDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public ClassInformationModel ClassInformation { get; set; } = new ClassInformationModel();

    public List<ClassInformationTable> FilteredClasses { get; set; } = new List<ClassInformationTable>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id.HasValue)
        {
            var classToEdit = await _context.Classes.FindAsync(id.Value);
            if (classToEdit != null)
            {
                ClassInformation = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.Name,
                    StudentCount = classToEdit.PersonCount,
                    Description = classToEdit.Description
                };
            }
        }

        IQueryable<Class> query = _context.Classes.Where(c => c.IsActive);

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(SearchTerm));
        }

        var totalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
        PageNumber = Math.Max(1, Math.Min(PageNumber, TotalPages > 0 ? TotalPages : 1));

        var classes = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        FilteredClasses = classes.Select(c => new ClassInformationTable
        {
            Id = c.Id,
            ClassName = c.Name,
            StudentCount = c.PersonCount,
            Description = c.Description,
            IsActive = c.IsActive
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (ClassInformation.Id == 0)
        {
            var newClass = new Class
            {
                Name = ClassInformation.ClassName,
                PersonCount = ClassInformation.StudentCount,
                Description = ClassInformation.Description,
                IsActive = ClassInformation.IsActive
            };
            _context.Classes.Add(newClass);
        }
        else
        {
            var existingClass = await _context.Classes.FindAsync(ClassInformation.Id);
            if (existingClass != null)
            {
                existingClass.Name = ClassInformation.ClassName;
                existingClass.PersonCount = ClassInformation.StudentCount;
                existingClass.Description = ClassInformation.Description;
                existingClass.IsActive = ClassInformation.IsActive;
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Dashboard");
    }

    public IActionResult OnPostEdit(int id)
    {
        return RedirectToPage("./Dashboard", new { id = id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var classToUpdate = await _context.Classes.FindAsync(id);
        if (classToUpdate != null)
        {
            classToUpdate.IsActive = false; // Soft delete by deactivating
            await _context.SaveChangesAsync();
        }
        return RedirectToPage("./Dashboard");
    }

    public async Task<IActionResult> OnPostExportAsync(bool filteredOnly, string[] selectedColumns, string? searchTerm, int pageNumber)
    {
        IQueryable<Class> query = _context.Classes.AsQueryable();

        if (filteredOnly)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();
            int validPage = Math.Max(1, Math.Min(pageNumber, (int)Math.Ceiling((double)totalCount / PageSize)));
            query = query.Skip((validPage - 1) * PageSize).Take(PageSize);
        }

        var dataToExport = await query.Select(c => new ClassInformationTable
        {
            Id = c.Id,
            ClassName = c.Name,
            StudentCount = c.PersonCount,
            Description = c.Description,
            IsActive = c.IsActive
        }).ToListAsync();

        var json = Utils.Instance.ExportToJson(dataToExport, selectedColumns?.Any() == true ? selectedColumns : null);

        var logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Log");
        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }

        var fileName = $"{(filteredOnly ? "filtered_classes" : "all_classes")}_{DateTime.Now:yyyyMMddHHmmss}.json";
        var filePath = Path.Combine(logFolderPath, fileName);
        System.IO.File.WriteAllText(filePath, json);

        return new JsonResult(new { json, fileName });
    }
}