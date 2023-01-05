using MediatR;
using Microsoft.AspNetCore.Mvc;
using MockTracer.Test.Api.Application.Features.Topic;
using System.Net;

namespace MockTracer.Test.Api.Controllers;

[ApiController]
[Route("api/topic/v10")]
public class TopicController : Controller
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

  [HttpGet("sql-call")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> SqlCallAsync(int type)
  {
    if(type == 0)
    {
      return Ok(await _mediator.Send(new SqldDataComandHandler.SingleRow()));
    }
    else if(type == 1)
    {
      return Ok(await _mediator.Send(new SqldDataComandHandler.SystemDate()));
    }
    else if (type == 2)
    {
      return Ok(await _mediator.Send(new SqldDataComandHandler.MultupleQueryAsync()));
    }
    else if (type == 3)
    {
      return Ok(await _mediator.Send(new SqldDataComandHandler.UserList()));
    }
    else
    {
      return Ok(await _mediator.Send(new SqldDataComandHandler.ExecuteNonQuery()));
    }
  }

  [HttpGet("fact")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetfactAsync()
  {
    return Ok(await _mediator.Send(new CatFactQuery()));
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
}
