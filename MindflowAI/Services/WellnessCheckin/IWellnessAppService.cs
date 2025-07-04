using MindflowAI.Entities.WellnessCheckin;
using MindflowAI.Services.Dtos.WellnessCheckin;
using Volo.Abp.Application.Services;

namespace MindflowAI.Services.WellnessCheckin
{
    public interface IWellnessAppService : IApplicationService
    {
        Task SubmitAsync(WellnessCheckInDto input);
        Task<WellnessCheckIn?> GetAsync();

    }
}