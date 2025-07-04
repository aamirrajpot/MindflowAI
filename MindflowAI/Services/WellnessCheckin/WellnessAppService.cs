using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MindflowAI.Entities.WellnessCheckin;
using MindflowAI.Services.Dtos.WellnessCheckin;
using MindflowAI.Services.TaskSuggestion;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace MindflowAI.Services.WellnessCheckin
{
    public class WellnessAppService : ApplicationService, IWellnessAppService
    {
        private readonly IRepository<WellnessCheckIn, Guid> _repo;
        private readonly IHttpClientFactory _httpClientFactory;


        public WellnessAppService(IRepository<WellnessCheckIn, Guid> repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClientFactory = httpClientFactory;
        }
        [Authorize]
        public async Task SubmitAsync(WellnessCheckInDto input)
        {
            var userId = CurrentUser.Id ?? throw new UserFriendlyException("User not found");

            var checkIn = new WellnessCheckIn(
                GuidGenerator.Create(),
                userId,
                input.StressLevel,
                input.MoodLevel,
                input.EnergyLevel,
                input.SpiritualWellness,
                Clock.Now
            );

            await _repo.InsertAsync(checkIn, autoSave: true);

            // Trigger post-checkin evaluation
            //await EvaluateWellnessAndTriggerSuggestionsAsync(checkIn);
        }
        [Authorize]
        public async Task<WellnessCheckIn?> GetAsync()
        {
            var userId = CurrentUser.Id ?? throw new UserFriendlyException("User not found");
            return await (await _repo.GetQueryableAsync())
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CheckInDate)
                .FirstOrDefaultAsync();
        }
        private async Task EvaluateWellnessAndTriggerSuggestionsAsync(WellnessCheckIn checkIn)
        {
            var client = _httpClientFactory.CreateClient("Ollama");
            // Critical condition
            if (checkIn.StressLevel >= 8 || checkIn.MoodLevel <= 1)
            {
                await TaskSuggestionEngine.SuggestUrgentSupportAsync(checkIn.UserId);
            }

            // Spiritual category
            //if (checkIn.SpiritualWellness <= 3)
            //{
                await TaskSuggestionEngine.SuggestTasksAsync(checkIn, client);
            //}

            //// Emotional category
            //if (checkIn.MoodLevel <= 2)
            //{
            //    await TaskSuggestionEngine.SuggestTaskAsync(checkIn.UserId, "emotional");
            //}

            //// Mental category (high stress)
            //if (checkIn.StressLevel >= 6)
            //{
            //    await TaskSuggestionEngine.SuggestTaskAsync(checkIn);
            //}

            //// Physical category
            //if (checkIn.EnergyLevel <= 1)
            //{
            //    await TaskSuggestionEngine.SuggestTaskAsync(checkIn);
            //}
        }
    }
}
