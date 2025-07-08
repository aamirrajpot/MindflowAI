using MindflowAI.Services.Dtos.Google;
using Volo.Abp.Application.Services;

namespace MindflowAI.Services.Google
{
    public interface IGoogleAuthService : IApplicationService
    {
        Task<object> GoogleLoginAsync(GoogleLoginDto input);
    }
}