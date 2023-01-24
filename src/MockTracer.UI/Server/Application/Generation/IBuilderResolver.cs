namespace MockTracer.UI.Server.Application.Generation;

public interface IBuilderResolver
{
  InputPointBuilderBase ResolveInputBuilder(string tracerCode);

  ITemplateBuilder ResolveTemplateBuilder(string templateCode);

  MockPointBuilderBase ResolveMockBuilder(string tracerCode);
}
