using MockTracer.UI.Server.Application.Presentation;
using MockTracer.UI.Server.Options;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Application.Generation;

public interface ITemplateBuilder
{
  string Build(GenerationAttributes @params, ClassGenerationSetting options, GenerationContext data);
}
