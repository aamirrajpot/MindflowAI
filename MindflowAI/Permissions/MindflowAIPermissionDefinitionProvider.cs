using MindflowAI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MindflowAI.Permissions;

public class MindflowAIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MindflowAIPermissions.GroupName);


        var booksPermission = myGroup.AddPermission(MindflowAIPermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(MindflowAIPermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(MindflowAIPermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(MindflowAIPermissions.Books.Delete, L("Permission:Books.Delete"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MindflowAIPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MindflowAIResource>(name);
    }
}
