using Volo.Abp.Domain.Entities.Auditing;

namespace MindflowAI.Entities.WellnessCheckin
{
    public class WellnessCheckIn : AuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public int StressLevel { get; set; }          // e.g., 1–10
        public int MoodLevel { get; set; }            // e.g., 1:Sad, 2:Neutral, 3:Happy
        public int EnergyLevel { get; set; }          // 1:Low, 2:Medium, 3:High
        public int SpiritualWellness { get; set; }    // 1–10

        public DateTime CheckInDate { get; set; }

        public WellnessCheckIn() { }

        public WellnessCheckIn(
            Guid id,
            Guid userId,
            int stress,
            int mood,
            int energy,
            int spiritual,
            DateTime checkInDate) : base(id)
        {
            UserId = userId;
            StressLevel = stress;
            MoodLevel = mood;
            EnergyLevel = energy;
            SpiritualWellness = spiritual;
            CheckInDate = checkInDate;
        }
    }
}
