using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MyRazorApp.Helpers;
using System.Text.Json;
using System.Linq; // Add this for FirstOrDefault

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
    private static List<ClassInformationModel> classInformations = new List<ClassInformationModel>();
    private static int nextId = 1;

    [BindProperty]
    public ClassInformationModel ClassInformation { get; set; } = new ClassInformationModel();

    public List<ClassInformationTable> FilteredClasses { get; set; } = new List<ClassInformationTable>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }

    public void OnGet(int? id) // Make the id parameter nullable
    {
        // Ensure synthetic data is present for pagination testing
        if (!classInformations.Any())
        {
            GenerateSyntheticData();
        }

        // Check if an ID is passed for editing
        if (id.HasValue)
        {
            var classToEdit = classInformations.FirstOrDefault(c => c.Id == id.Value);
            if (classToEdit != null)
            {
                ClassInformation = classToEdit;
            }
        }

        // Filtering
        var filteredList = string.IsNullOrEmpty(SearchTerm)
            ? classInformations
            : classInformations.Where(c => c.ClassName.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase)).ToList();

        // Pagination
        TotalPages = (int)System.Math.Ceiling((double)filteredList.Count / PageSize);
        PageNumber = System.Math.Max(1, System.Math.Min(PageNumber, TotalPages > 0 ? TotalPages : 1)); // Ensure valid page number

        FilteredClasses = filteredList
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            })
            .ToList();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (ClassInformation.Id == 0)
        {
            ClassInformation.Id = nextId++;
            classInformations.Add(ClassInformation);
        }
        else
        {
            var existingItem = classInformations.Find(x => x.Id == ClassInformation.Id);
            if (existingItem != null)
            {
                existingItem.ClassName = ClassInformation.ClassName;
                existingItem.StudentCount = ClassInformation.StudentCount;
                existingItem.Description = ClassInformation.Description;
            }
        }

        return RedirectToPage("./Index");
    }

    public IActionResult OnPostEdit(int id)
    {
        return RedirectToPage("./Index", new { id = id });
    }

    public IActionResult OnPostDelete(int id)
    {
        var itemToRemove = classInformations.Find(x => x.Id == id);
        if (itemToRemove != null)
        {
            classInformations.Remove(itemToRemove);
        }

        return RedirectToPage("./Index");
    }

    private void GenerateSyntheticData()
    {
        for (int i = 1; i <= 100; i++)
        {
            classInformations.Add(new ClassInformationModel
            {
                Id = nextId++,
                ClassName = $"Class {i}",
                StudentCount = new System.Random().Next(20, 50),
                Description = $"Description for Class {i}"
            });
        }
    }

    public IActionResult OnPostExport(bool filteredOnly, string[] selectedColumns, string? searchTerm, int pageNumber)
{
    IEnumerable<ClassInformationTable> dataToExport;

    if (filteredOnly)
    {
        // Filter
        var filteredList = string.IsNullOrEmpty(searchTerm)
            ? classInformations
            : classInformations.Where(c => c.ClassName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

        // Pagination
        int totalPages = (int)Math.Ceiling((double)filteredList.Count / PageSize);
        int validPage = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

        var pagedList = filteredList
            .Skip((validPage - 1) * PageSize)
            .Take(PageSize)
            .Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            });

        dataToExport = pagedList;
    }
    else
    {
        dataToExport = classInformations.Select(c => new ClassInformationTable
        {
            Id = c.Id,
            ClassName = c.ClassName,
            StudentCount = c.StudentCount,
            Description = c.Description
        });
    }

    var json = Utils.Instance.ExportToJson(dataToExport, selectedColumns?.Any() == true ? selectedColumns : null);
    return new JsonResult(new { json });
}

}