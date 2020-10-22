using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Demo.Data
{
  public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Demo.Models.Project> Project { get; set; }
  }
}
