namespace MockTracer.UI.Server.Application.Generation;

public interface IBuilderResolver
{
  TracerBuilderBase ResolveInputBuilder(string tracerCode);

  ITemplateBuilder ResolveTemplateBuilder(string templateCode);

  MockBuilderBase ResolveMockBuilder(string tracerCode);
}
