using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Pages;

public class ClassInformationModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Class Name is required.")]
    public string ClassName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Student Count is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Student Count must be greater than 0.")]
    public int StudentCount { get; set; }

    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}