using Volo.Abp.Identity;

namespace MindflowAI.Entities.AppUser
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public AppUser(Guid id, string userName, string email)
            : base(id, userName, email)
        {
        }
    }
}
