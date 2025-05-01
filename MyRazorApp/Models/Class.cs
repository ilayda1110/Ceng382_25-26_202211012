using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyRazorApp.Models;

[Table("Classes")]
public partial class Class
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int PersonCount { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
