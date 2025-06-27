namespace MindflowAI.Services.Dtos.AppUser
{
    public class OtpVerificationDto
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
