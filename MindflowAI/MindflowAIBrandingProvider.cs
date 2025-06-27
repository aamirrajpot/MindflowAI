using Microsoft.Extensions.Localization;
using MindflowAI.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MindflowAI;

[Dependency(ReplaceServices = true)]
public class MindflowAIBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MindflowAIResource> _localizer;

    public MindflowAIBrandingProvider(IStringLocalizer<MindflowAIResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}