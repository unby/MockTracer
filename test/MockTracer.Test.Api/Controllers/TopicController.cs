using MediatR;
using Microsoft.AspNetCore.Mvc;
using MockTracer.Test.Api.Application.Features.HTTP;
using MockTracer.Test.Api.Application.Features.SQL;
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
  /// SingleRow
  /// </summary>
  /// <returns></returns>
  [HttpGet("sql-call/SingleRow")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlSingleRowAsync()
  {
    return Ok(await _mediator.Send(new SqldDataComandHandler.SingleRow()));
  }

  /// <summary>
  /// ExecuteNonQuery
  /// </summary>
  /// <returns></returns>
  [HttpGet("sql-call/ExecuteNonQuery")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlExecuteNonQueryAsync()
  {
    return Ok(await _mediator.Send(new SqldDataComandHandler.ExecuteNonQuery()));
  }

  /// <summary>
  /// SystemDate
  /// </summary>
  /// <returns></returns>
  [HttpGet("sql-call/SystemDate")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlSystemDateAsync()
  {
    return Ok(await _mediator.Send(new SqldDataComandHandler.SystemDate()));
  }

  /// <summary>
  /// MultupleQueryAsync
  /// </summary>
  /// <returns></returns>
  [HttpGet("sql-call/MultupleQueryAsync")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlMultupleQueryAsync()
  {
    return Ok(await _mediator.Send(new SqldDataComandHandler.MultupleQueryAsync()));
  }

  /// <summary>
  /// UserList
  /// </summary>
  /// <returns></returns>
  [HttpGet("sql-call/UserList")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlUserListAsync()
  {
    return Ok(await _mediator.Send(new SqldDataComandHandler.UserList()));
  }

  /// <summary>
  /// NewUserCommand
  /// </summary>
  /// <returns></returns>
  [HttpGet("sql-call/NewUserCommand")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> NewUserCommandAsync()
  {
    return Ok(await _mediator.Send(new SqldDataComandHandler.NewUserCommand()));
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
