using MindflowAI.Utilities;
using System.Text.Json.Serialization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace MindflowAI.Services.Ollama
{
    public class OllamaService : ApplicationService, IOllamaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OllamaService> _logger;

        public OllamaService(IHttpClientFactory httpClientFactory, ILogger<OllamaService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<AiAnalysisResult?> Analyze(string journalText)
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            var prompt = LlamaPromptBuilder.BuildPrompt(journalText);

            var request = new
            {
                model = "llama2",
                prompt = prompt,
                stream = false // <-- IMPORTANT!
            };

            var response = await client.PostAsJsonAsync("/api/generate", request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Ollama API failed: {StatusCode}", response.StatusCode);
                throw new BusinessException("OllamaError").WithData("StatusCode", response.StatusCode);
            } 

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            return OllamaResponseParser.Parse(result.Response);

        }

        
    }

}
