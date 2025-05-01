using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages;


public class ClassInformationTable
{
    public int Id { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}