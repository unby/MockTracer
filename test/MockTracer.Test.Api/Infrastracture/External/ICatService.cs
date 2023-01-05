using MockTracer.Test.Api.Domain;
using Refit;

namespace MockTracer.Test.Api.Infrastracture.External;

public interface ICatService
{
  // https://catfact.ninja/fact
  [Get("/fact")]
  Task<CatFact> GetCatFactAsync();
}
