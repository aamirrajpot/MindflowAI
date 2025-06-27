using Volo.Abp.Application.Services;
using MindflowAI.Localization;

namespace MindflowAI.Services;

/* Inherit your application services from this class. */
public abstract class MindflowAIAppService : ApplicationService
{
    protected MindflowAIAppService()
    {
        LocalizationResource = typeof(MindflowAIResource);
    }
}