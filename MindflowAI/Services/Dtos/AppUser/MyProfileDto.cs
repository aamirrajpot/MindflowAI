namespace MindflowAI.Services.Dtos.AppUser
{
    public class MyProfileDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
