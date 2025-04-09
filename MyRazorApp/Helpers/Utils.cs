/*

Do this:
Add a new button to export the data to JSON. There should be two modes:
o Unfiltered export (exports the entire data)
o Filtered export (exports only the currently filtered rows)
• Also add the ability to select specific columns for export:
o If no column is selected, export all columns.
o If certain columns (e.g., 1st and 4th) are selected, export only those columns.
o The selected columns should visually change color to indicate selection.
o The exported JSON should contain only the selected column data.
4. Utility Class for JSON Export
• Create a new C# class file named Utils.cs.
• Inside it, implement a generic method that can export any class to JSON.
• The method should work with any model class.
• This class must be implemented as a singleton, so it can be accessed from anywhere in the
project.
5. Folder Structure Reminder (MVP)
Since your project follows the MVP structure in a Razor Pages application:
• Place the ClassInformationTable and related data models in the Models folder.
• Place the Utils.cs class in a separate folder called Helpers or Utilities.
• Place pagination logic, filtering logic, and UI-related code in the appropriate Pages folder.

(source code provided. My Index.cshtml, Index.cshtml.cs, ClassInformationModel.cs, ClassInformationTable.cs)



Gives error as "Type of conditional expression cannot be determined because there is no implicit conversion between 'System.Collections.Generic.List<MyRazorApp.Pages.ClassInformationTable>' and 'System.Collections.Generic.List<MyRazorApp.Pages.ClassInformationModel>" for "var data = filteredOnly ? FilteredClasses : classInformations;" in OnPostExport



When i press "Edit", it gets the chosen class' infos and let user to change whatever they want to change. But when user presses Submit, it just reloads the page and do nothing. The code u sent is still not my old one



(source code provided. My Index.cshtml)
Add necessary things so that it works for JSON files



(source code provided. My Index.cshtml, Index.cshtml.cs, Utils.cs)
I want my "Export Filtered" to write the current page of the table to a json file



Right now my "Export All" button writes all data to json file (which is what i want. do not change that) but my "Export Filtered" doesnt work.  I want it to write current page of the table to a json file



(source code provided. My Index.cshtml, Index.cshtml.cs, Utils.cs)
When pressed "Export" i want it to download/create a json file inside of a folder named "Log" not just download. I will create that "Log" folder under MyRazorApp
*/


// Helpers/Utils.cs
using System.Text.Json;

namespace MyRazorApp.Helpers;

public sealed class Utils
{
    private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());
    
    public static Utils Instance => _instance.Value;
    
    private Utils() { }
    
    public string ExportToJson<T>(IEnumerable<T> data, IEnumerable<string>? selectedColumns = null)
{
    if (selectedColumns != null && selectedColumns.Any())
    {
        // Handle both concrete types and anonymous types
        var filteredData = data.Select(item =>
        {
            var dict = new Dictionary<string, object?>();
            var type = item.GetType();
            
            foreach (var prop in type.GetProperties())
            {
                if (selectedColumns.Contains(prop.Name))
                {
                    dict[prop.Name] = prop.GetValue(item);
                }
            }
            return dict;
        });
        
        return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
    }
    
    return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
}
}