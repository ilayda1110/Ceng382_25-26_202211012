using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages;

/*

Inside this folder, create a class named ClassInformationModel.cs.
• This class will store the following properties:
o Id (auto-incremented)
o ClassName
o StudentCount
o Description
The Id property will be automatically incremented each time a new item is added to the list. The list
will act like a simple in-memory database.

For my Razor Page project i need to do this. What to write in ClassInformationModel.cs?

*/


/*
On the left side of the page, there will be a form that collects:
o Class Name
o Student Count
o Description
• On the right side, there will be a table that displays all the submitted class data.
• The table will have the following columns:
o Id
o Class Name
o Student Count
o Description
o Actions (Edit and Delete)
The data should be validated and added to a static list each time the form is submitted. The data will
then be displayed in the table.

Now do this
*/

/*
Use Bootstrap to create a responsive layout with two columns (form on the left, table on the
right).
• Use Razor Pages only; no JavaScript is allowed.
• All operations (Add, Edit, Delete) must be handled using C# methods in the PageModel.
• Form validation should be done using C# attributes like [Required], [Range], etc.
• When editing, pre-fill the form with the selected item's data.
• After deletion or editing, refresh the page and update the table accordingly.

Add these now
*/


public class ClassInformationModel
{
    public int Id { get; set; } // Assigned in AddClass()

    [Required(ErrorMessage = "Class Name is required.")]
    public string ClassName { get; set; }

    [Required(ErrorMessage = "Student Count is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Student Count must be greater than 0.")]
    public int StudentCount { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; }
}

public static class ClassInformationDatabase
{
    private static List<ClassInformationModel> _classes = new List<ClassInformationModel>();
    private static int _nextId = 1; // Centralized auto-increment ID

    public static void AddClass(ClassInformationModel classInfo)
    {
        classInfo.Id = _nextId++;
        _classes.Add(classInfo);
    }

    public static List<ClassInformationModel> GetAllClasses()
    {
        return new List<ClassInformationModel>(_classes);
    }

    public static void UpdateClass(ClassInformationModel updatedClass)
    {
        var existingClass = _classes.Find(c => c.Id == updatedClass.Id);
        if (existingClass != null)
        {
            existingClass.ClassName = updatedClass.ClassName;
            existingClass.StudentCount = updatedClass.StudentCount;
            existingClass.Description = updatedClass.Description;
        }
    }

    public static void DeleteClass(int id)
    {
        _classes.RemoveAll(c => c.Id == id);
    }

    public static ClassInformationModel GetClassById(int id)
    {
        return _classes.Find(c => c.Id == id);
    }
}