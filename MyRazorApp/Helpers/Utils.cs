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