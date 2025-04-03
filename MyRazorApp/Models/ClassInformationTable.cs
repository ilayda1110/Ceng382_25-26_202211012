using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages;

/* First prompt i used for week6 tasks:

I created a Razor Pages project. I have a ClassList table which displays Id, ClassName, StudentCount, Description and Actions(Edit or Delete) and Add/Edit Class table to get input from user and a submit button.
Now i want to add filtering and pagination features. Filtering will be done on the data list in the backend and will be written inside of OnGet methods.
You will also create a new model class called ClassInformationTable. This model will store
the filtered version of your main model and will be used to display data in the table. In this
model, the ID should not be shown in the table, but the ID will still be used in the
background for actions like edit, delete, or details.
In addition to filtering, you are required to implement pagination. To properly test the
pagination feature, you need to generate synthetic data. Make sure to create a list with at
least 100 sample records so you can see how the pagination works across multiple pages.
 Tip :
When a filter value changes, the form should submit automatically or the user should click a
"Filter" button. This will trigger the OnGet method with the selected filter values passed as
query parameters.
(source code provided Index.cshtml, Index.cshtml.cs, ClassInformationModel.cs)

*/

public class ClassInformationTable
{
    public string ClassName { get; set; }
    public int StudentCount { get; set; }
    public string Description { get; set; }
    public int Id { get; set; } // This will be used internally but not displayed
}