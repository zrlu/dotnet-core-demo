using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.ViewModels
{
  public class ProjectViewModel
  {
    [Required]
    [Display(Name = "Project ID")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Project Name")]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Project Status")]
    public string Status { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; }

  }
}
