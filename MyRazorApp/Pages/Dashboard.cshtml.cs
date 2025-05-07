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