using Volo.Abp.Domain.Entities;

namespace MindflowAI.Entities.EmailOtp
{
    public class EmailOtp : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }

        public EmailOtp(Guid id, Guid userId, string code, DateTime expirationTime)
            : base(id)
        {
            UserId = userId;
            Code = code;
            ExpirationTime = expirationTime;
            IsUsed = false;
        }
    }
}
