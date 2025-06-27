using Volo.Abp.Application.Services;
using static MindflowAI.Services.Ollama.OllamaService;

namespace MindflowAI.Services.Ollama
{
    public interface IOllamaService : IApplicationService
    {
        Task<AiAnalysisResult> Analyze(string journalText);
    }
}