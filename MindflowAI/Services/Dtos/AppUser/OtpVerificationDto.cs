namespace MindflowAI.Services.Dtos.AppUser
{
    public class OtpVerificationDto
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public string Password { get; set; } // Needed to log in with password grant

    }
}
