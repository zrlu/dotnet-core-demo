using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo.Data;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Demo.Infrastructures;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;
using Demo.ViewModels;
using System.Linq.Expressions;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net;

namespace Demo.Controllers
{
  public class ProjectsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer<ProjectsController> _localizer;
    private readonly AppUserManager _userManager;
    private readonly log4net.ILog _log;


    public ProjectsController(ApplicationDbContext context, IStringLocalizer<ProjectsController> localizer, AppUserManager userManager)
    {
      _context = context;
      _localizer = localizer;
      _userManager = userManager;
      _log = LogManager.GetLogger(typeof(ProjectsController));
    }

    // GET: Projects
    [Authorize]
    public async Task<IActionResult> Index(int pageIndex)
    {
      _log.Info("Index");
      AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
      Expression<Func<Project, bool>> predicate;
      if (await _userManager.IsInRoleAsync(currentUser, "Administrator"))
      {
        predicate = p => p.IsActive;
      } else
      {
        predicate = p => p.IsActive && p.UserId == currentUser.Id;
      }

      var applicationDbContext = _context.Project.Include(p => p.User);
      var query = _context.Project.Where(predicate)
        .Join(
          _context.Users,
          p => p.UserId,
          u => u.Id,
          (p, u) => new ProjectViewModel { Id = p.Id, Name = p.Name, Status = p.Status, CreatedBy = u.UserName }
        );
      PagedResults<ProjectViewModel> pagedResults = await PagedResults<ProjectViewModel>.GetPagedListAsync(query, pageIndex, 10);
      ViewData["PageCount"] = pagedResults.PageCount;
      ViewData["RowCount"] = pagedResults.RowCount;
      ViewData["CurrentPage"] = pagedResults.CurrentPage;
      ViewData["PageSize"] = pagedResults.PageSize;
      return View(pagedResults.Results);
    }

    // GET: Projects/Details/5
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var project = await _context.Project
          .Include(p => p.User)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (project == null)
      {
        return NotFound();
      }

      AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
      if (!await _userManager.IsInRoleAsync(currentUser, "Administrator") && project.UserId != currentUser.Id)
      {
        return Unauthorized();
      }

      var user = await _context.Users
          .Where(u => u.Id == project.UserId)
          .FirstOrDefaultAsync();
      ProjectViewModel projectVM = new ProjectViewModel
      {
        Id = project.Id,
        Name = project.Name,
        Status = project.Status,
        CreatedBy = user.UserName
      };
      return View(projectVM);
    }

    // GET: Projects/Create
    [Authorize]
    public IActionResult Create()
    {
      return View();
    }

    // POST: Projects/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Status")] ProjectViewModel projectVM)
    {
      if (ModelState.IsValid)
      {
        AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
        _context.Project.Add(new Project
        {
          UserId = currentUser.Id,
          Name = projectVM.Name,
          Status = projectVM.Status,
          IsActive = true,
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(projectVM);
    }

    // GET: Projects/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var project = await _context.Project.FindAsync(id);
      if (project == null)
      {
        return NotFound();
      }

      AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
      if (!await _userManager.IsInRoleAsync(currentUser, "Administrator") && project.UserId != currentUser.Id)
      {
        return Unauthorized();
      }

      ProjectViewModel projectVM = new ProjectViewModel
      {
        Id = project.Id,
        Name = project.Name,
        Status = project.Status,
        CreatedBy = currentUser.UserName
      };
      return View(projectVM);
    }

    // POST: Projects/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Status")] ProjectViewModel projectVM)
    {
      Project project = await _context.Project.Where(p => p.Id == id).FirstOrDefaultAsync();
      if (project == null)
      {
        return NotFound();
      }

      AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
      if (!await _userManager.IsInRoleAsync(currentUser, "Administrator") && project.UserId != currentUser.Id)
      {
        return Unauthorized();
      }

      if (ModelState.IsValid)
      {
        try
        {
          project.Name = projectVM.Name;
          project.Status = projectVM.Status;
          _context.Update(project);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ProjectExists(id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(projectVM);
    }

    [Authorize]
    // GET: Projects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var project = await _context.Project
          .Include(p => p.User)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (project == null)
      {
        return NotFound();
      }

      AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
      if (!await _userManager.IsInRoleAsync(currentUser, "Administrator") && project.UserId != currentUser.Id)
      {
        return Unauthorized();
      }

      ProjectViewModel projectVM = new ProjectViewModel
      {
        Id = project.Id,
        Name = project.Name
      };
      return View(projectVM);
    }

    // POST: Projects/Delete/5
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var project = await _context.Project.FindAsync(id);

      AppUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
      if (!await _userManager.IsInRoleAsync(currentUser, "Administrator") && project.UserId != currentUser.Id)
      {
        return Unauthorized();
      }

      //_context.Project.Remove(project);
      project.IsActive = false;
      _context.Update(project);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool ProjectExists(int id)
    {
      return _context.Project.Any(e => e.Id == id);
    }
  }
}
