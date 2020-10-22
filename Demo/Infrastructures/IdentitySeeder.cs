using Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Infrastructures
{
  public class IdentitySeeder
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentitySeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }

    public async Task<bool> CreateRoleIfNotExist(string roleName)
    {
      bool exist = await _roleManager.RoleExistsAsync(roleName);
      if (!exist)
      {
        IdentityResult result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
        return result.Succeeded;
      }
      return false;
    }

    public async Task<bool> CreateAdminIfNotExist(string adminEmail, IEnumerable<string> roleNames, string defaultPassword)
    {
      var admin = await _userManager.FindByNameAsync(adminEmail);
      if (admin == null)
      {
        IdentityResult result = await _userManager.CreateAsync(new AppUser { UserName = adminEmail, Email = adminEmail });
        if (result.Succeeded)
        {
          admin = await _userManager.FindByNameAsync(adminEmail);
          result = await _userManager.AddPasswordAsync(admin, defaultPassword);
          if (result.Succeeded)
          {
            result = await _userManager.AddToRolesAsync(admin, roleNames);
            return result.Succeeded;
          }
        }
      }
      return false;
    }
    public async Task<bool> SeedAllAsync()
    {
      bool adminRoleCreated = await CreateRoleIfNotExist("Administrator");
      bool adminCreated = await CreateAdminIfNotExist("admin@example.com", new string[] { "Administrator" }, "password");
      return adminRoleCreated && adminCreated;
    }

    public bool SeedAll()
    {
      Task<bool> task = SeedAllAsync();
      task.Wait();
      return task.Result;
    }
  }
}
