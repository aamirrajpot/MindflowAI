
using Volo.Abp.Application.Services;

namespace MindflowAI.Services.User
{
    public interface IUserAccountAppService : IApplicationService
    {
        Task<Guid> RegisterWithOtpAsync(string email, string password);
        Task VerifyOtpAsync(Guid userId, string code);
    }
}