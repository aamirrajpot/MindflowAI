
using MindflowAI.Services.Dtos.AppUser;
using Volo.Abp.Application.Services;

namespace MindflowAI.Services.User
{
    public interface IUserAccountAppService : IApplicationService
    {
        Task<Guid> RegisterAsync(RegisterWithOtpDto input);
        Task VerifyOtpAsync(OtpVerificationDto input);
        Task<bool> ChangePasswordAsync(ChangePasswordDto input);
        Task<MyProfileDto> GetMyProfileAsync();
        Task UpdateMyProfileAsync(UpdateMyProfileDto input);
    }
}