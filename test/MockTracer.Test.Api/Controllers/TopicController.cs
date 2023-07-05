using MediatR;
using Microsoft.AspNetCore.Mvc;
using MockTracer.Test.Api.Application.Features.HTTP;
using MockTracer.Test.Api.Application.Features.Topic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MockTracer.Test.Api.Controllers;

[ApiController]
[Route("api/topic/v10")]
public class TopicController : ControllerBase
{
  private readonly IMediator _mediator;

  public TopicController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet()]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetTopicsAsync()
  {
    return Ok(await _mediator.Send(new TopicQuery()));
  }

  /// <summary>
  /// SingleRow, SystemDate, MultupleQueryAsync, UserList, ExecuteNonQuery
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  [HttpGet("sql-call")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlCallAsync([Required] string type)
  {
    return Ok(await _mediator.Send(ResolveHandler("SqldDataComandHandler", type)));
  }


  [HttpGet("fact")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetfactAsync()
  {
    return Ok(await _mediator.Send(new CatFactQuery()));
  }

  [HttpGet("mediator-exception")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> MediatorExceptionAsync()
  {
    return Ok(await _mediator.Send(new ExceptionQuery()));
  }
  

  [HttpGet("multiple-query")]
  [ProducesResponseType((int)HttpStatusCode.Accepted)]
  public async Task<IActionResult> MultipleQueryAsync([FromQuery] MultipleQuery request)
  {
    return Ok(await _mediator.Send(request));
  }

  [HttpPost]
  [ProducesResponseType((int)HttpStatusCode.Created)]
  public async Task<IActionResult> CreateArticle([FromBody] CreateTopicCommand request)
  {
    return Ok(await _mediator.Send(request));
  }

  private object ResolveHandler(string type, string nestedName = null)
  {
    try
    {
      if (nestedName != null)
      {
        var n = GetType().Assembly.GetTypes().Where(w=>w.Name==type).SelectMany(s => s.GetNestedTypes()).FirstOrDefault(f=>f.Name == nestedName);
        return Activator.CreateInstance(n);
      }
      else
      {
        return Activator.CreateInstance(GetType().Assembly.FullName, type);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw;
    }
  }
}
