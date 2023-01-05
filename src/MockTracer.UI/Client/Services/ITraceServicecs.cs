using MockTracer.UI.Shared.Data;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Client.Services;

public interface ITraceService
{
  Task<PagedResult<StackScope>> GetTraceListAsync(int page);

  Task<StackRow[]> GetTraceRowsAsync(Guid scopeId);

  Task MakeTestAsync(GenerationAttributes attributes);

  Task MakeInternalTestAsync(GenerationAttributes attributes);
}
