using Microsoft.EntityFrameworkCore;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Data;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Application.Presentation;

public class TraceRepository
{
  private readonly MockTracerDbContext _context;

  public TraceRepository(MockTracerDbContext context)
  {
    _context = context;
  }

  public async Task<PagedResult<StackScope>> GetTracingAsync(int page)
  {
    int pageSize = 25;

    var result = new PagedResult<StackScope>();
    result.CurrentPage = page;
    result.PageSize = pageSize;
    result.RowCount = _context.StackScopes.Count();

    var pageCount = (double)result.RowCount / pageSize;
    result.PageCount = (int)Math.Ceiling(pageCount);

    var skip = (page - 1) * pageSize;
    result.Results = await _context.StackScopes
                   .OrderByDescending(p => p.Time).Skip(skip).Take(pageSize).ToListAsync();

    return result;
  }

  public Task<StackRow[]> GetRowsAsync(Guid scopeId)
  {
    return _context.StackRows.Where(w => w.ScopeId == scopeId)
      .Include(i => i.Exception).Include(i => i.Input).Include(i => i.Output).ToArrayAsync();
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
