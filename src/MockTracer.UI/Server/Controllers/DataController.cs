using System.Text;
using Microsoft.AspNetCore.Mvc;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Generation.IntenalTest;
using MockTracer.UI.Server.Application.Presentation;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
  private readonly TraceRepository _traceRepository;
  private readonly TestClassGenerator _generator;
  private readonly InternalTestClassGenerator _internalTestClassGenerator;

  public DataController(TraceRepository traceRepository, TestClassGenerator generator, InternalTestClassGenerator internalTestClassGenerator)
  {
    _traceRepository = traceRepository;
    _generator = generator;
    _internalTestClassGenerator = internalTestClassGenerator;
  }

  [HttpGet("trace-list")]
  public async Task<ActionResult> GetListAsync(int page)
  {
    return Ok(await _traceRepository.GetTracingAsync(page));
  }

  [HttpGet("trace-rows/{scopeId}")]
  public async Task<ActionResult> GetTraceAsync(Guid scopeId)
  {
    return Ok(await _traceRepository.GetRowsAsync(scopeId));
  }

  [HttpPost("generate")]
  public async Task<ActionResult> GenerateTestAsync(GenerationAttributes @params)
  {
    var result = await _generator.CreateAsync(@params);
    var mimeType = "text/plain";

    return new FileContentResult(Encoding.UTF8.GetBytes(result.SourceCode), mimeType)
    {
      FileDownloadName = result.FileName
    };
  }

  [HttpPost("internal-test-generate")]
  public async Task<ActionResult> UnitTestAsync(GenerationAttributes @params)
  {
    var result = await _internalTestClassGenerator.MakeTestAsync(@params);
    var mimeType = "text/plain";

    return new FileContentResult(Encoding.UTF8.GetBytes(result.SourceCode), mimeType)
    {
      FileDownloadName = result.FileName
    };
  }
}
