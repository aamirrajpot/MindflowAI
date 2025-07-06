using Volo.Abp.ObjectExtending;

namespace MindflowAI.Data
{
    public static class MindflowAIModuleExtensionConfigurator
    {
        public static void Configure()
        {
            ObjectExtensionManager.Instance
                .Modules()
                .ConfigureIdentity(identity =>
                {
                    identity.ConfigureUser(user =>
                    {
                        user.AddOrUpdateProperty<string>("FirstName");
                        user.AddOrUpdateProperty<string>("LastName");
                        user.AddOrUpdateProperty<DateTime?>("DateOfBirth");
                    });
                });
        }
    }

}
