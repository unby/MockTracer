using System.Runtime.Serialization;

namespace MockTracer.Test.Api.Domain.Exceptions;

public class ProcessingException : DomainException
{
  public ProcessingException() : base()
  {
  }

  public ProcessingException(string? message) : base(message)
  {
  }

  public ProcessingException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  public ProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
