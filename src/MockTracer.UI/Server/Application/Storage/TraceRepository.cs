using Microsoft.EntityFrameworkCore;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Data;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Application.Presentation;

/// <summary>
/// Mocktracer data source
/// </summary>
public class TraceRepository
{
  private readonly MockTracerDbContext _context;
  private const int PageSize = 25;

  /// <summary>
  /// TraceRepository
  /// </summary>
  /// <param name="context"><see cref="MockTracerDbContext"/></param>
  public TraceRepository(MockTracerDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// saved data
  /// </summary>
  /// <param name="page">page number</param>
  /// <returns>filtered list</returns>
  public async Task<PagedResult<StackScope>> GetTracingAsync(int page)
  {
    var result = new PagedResult<StackScope>();
    result.CurrentPage = page;
    result.PageSize = PageSize;
    result.RowCount = _context.StackScopes.Count();

    var pageCount = (double)result.RowCount / PageSize;
    result.PageCount = (int)Math.Ceiling(pageCount);

    var skip = (page - 1) * PageSize;
    result.Results = await _context.StackScopes
                   .OrderByDescending(p => p.Time).Skip(skip).Take(PageSize).ToListAsync();

    return result;
  }

  /// <summary>
  /// get StackScope
  /// </summary>
  /// <param name="scopeId">scopeId</param>
  /// <returns>finded StackScope</returns>
  public async Task<StackScope?> GetScopeAsync(Guid scopeId)
  {
    return await _context.StackScopes.Include(s => s.Stack).ThenInclude(k => k.Input)
              .Include(s => s.Stack).ThenInclude(s => s.Output)
              .Include(n => n.Stack).ThenInclude(e => e.Exception).FirstOrDefaultAsync(w => w.Id == scopeId);
  }

  internal async Task<GenerationContext> GetGenerationDataAsync(GenerationAttributes @params)
  {
    var context = new GenerationContext();
    context.Input = await _context.StackRows.Where(w => w.ScopeId == @params.ScopeId && (w.Id == @params.InputId))
      .Include(i => i.Exception).Include(i => i.Input).Include(i => i.Output).FirstAsync();
    if (@params.OutputId != null && @params.OutputId.Any())
    {
      context.Output = await _context.StackRows.Where(w => w.ScopeId == @params.ScopeId && @params.OutputId.Contains(w.Id))
        .Include(i => i.Exception).Include(i => i.Input).Include(i => i.Output).ToArrayAsync();
    }

    return context;
  }
}
