using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
        private static List<ClassInformationModel> classInformations = new List<ClassInformationModel>();
        private static int nextId = 1;

        [BindProperty]
        public ClassInformationModel ClassInformation { get; set; }

        public List<ClassInformationModel> ClassInformations => classInformations;

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                var item = classInformations.Find(x => x.Id == id.Value);
                if (item != null)
                {
                    ClassInformation = new ClassInformationModel
                    {
                        Id = item.Id,
                        ClassName = item.ClassName,
                        StudentCount = item.StudentCount,
                        Description = item.Description
                    };
                }
                else
                {
                    ClassInformation = new ClassInformationModel();
                }
            }
            else
            {
                ClassInformation = new ClassInformationModel();
            }
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
}