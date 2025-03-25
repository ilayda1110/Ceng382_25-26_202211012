using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

    public List<ClassInformationModel> Classes { get; private set; } = new List<ClassInformationModel>();

    [BindProperty]
    public int? EditId { get; set; } // Track which class is being edited

    public void OnGet()
    {
        Classes = ClassInformationDatabase.GetAllClasses();
    }

    public IActionResult OnPost()
{
    if (!ModelState.IsValid)
    {
        Classes = ClassInformationDatabase.GetAllClasses();
        return Page();
    }

    if (EditId.HasValue)
    {
        NewClass.Id = EditId.Value;
        ClassInformationDatabase.UpdateClass(NewClass);
        EditId = null;
        return RedirectToPage(); // Redirect to reload the page
    }
    else
    {
        ClassInformationDatabase.AddClass(NewClass);
        return RedirectToPage(); // Redirect to reload the page
    }
}

    public IActionResult OnPostEdit(int id)
    {
        var classToEdit = ClassInformationDatabase.GetClassById(id);
        if (classToEdit == null)
        {
            return RedirectToPage();
        }

        // Populate the form with class data for editing
        NewClass = new ClassInformationModel
        {
            Id = classToEdit.Id,
            ClassName = classToEdit.ClassName,
            StudentCount = classToEdit.StudentCount,
            Description = classToEdit.Description
        };

        EditId = id; // Set EditId to indicate we are editing this class
        Classes = ClassInformationDatabase.GetAllClasses();
        return Page(); // Return to the same page to show the populated form
    }

    public IActionResult OnPostDelete(int id)
    {
        ClassInformationDatabase.DeleteClass(id);
        return RedirectToPage(); // Refresh page after deletion
    }
}