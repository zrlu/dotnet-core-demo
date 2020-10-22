using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
  public class Project
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Status { get; set; }

    [ForeignKey("AppUser")]
    [Required]
    public string UserId { get; set; }

    public virtual AppUser User { get; set; }

    [DefaultValue(true)]
    [Required]
    public bool IsActive { get; set; }
  }
}
