using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Infrastructures
{
  public abstract class PagedResultsBase
  {
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }

  }

  public class PagedResults<TViewModel> : PagedResultsBase
  {
    public IList<TViewModel> Results { get; set; }

    public PagedResults()
    {
      Results = new List<TViewModel>();
    }

    public static async Task<PagedResults<TViewModel>> GetPagedListAsync(IQueryable<TViewModel> query, int pageIndex, int pageSize)
    {
      var results = new PagedResults<TViewModel>
      {
        PageSize = pageSize,
        RowCount = query.Count()
      };

      var pageCount = (double)results.RowCount / pageSize;

      results.PageCount = (int)Math.Ceiling(pageCount);
      if (results.PageCount == 0)
      {
        pageIndex = 1;
      }
      else
      {
        pageIndex = Math.Clamp(pageIndex, 1, results.PageCount);
      }
      results.CurrentPage = pageIndex;
      var skip = (pageIndex - 1) * pageSize;
      results.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

      return results;
    }
  }
}
