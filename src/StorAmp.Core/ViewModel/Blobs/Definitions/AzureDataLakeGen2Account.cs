using System.Threading.Tasks;
using Storage.Net;
using Storage.Net.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs.Gen2.Model;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Account;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Blobs.Definitions
{
   class AzureDataLakeGen2Account : BlobApplicationAccount
   {
      public AzureDataLakeGen2Account() : base(
         "blob",
         KnownPrefix.AzureDataLakeGen2,
         "Azure Data Lake (Gen 2)",
         new AccountConnectionType("sk", "Shared Key",
            new AccountField(KnownParameter.AccountName, "Account Name"),
            new AccountField(KnownParameter.KeyOrPassword, "Shared Key")),
         new AccountConnectionType("sp", "Service Principal",
            new AccountField(KnownParameter.AccountName, "Account Name"),
            new AccountField(KnownParameter.TenantId, "Tenant Id"),
            new AccountField(KnownParameter.ClientId, "Service Principal Id"),
            new AccountField(KnownParameter.ClientSecret, "Service Principal Secret"))
         )
      {
         AddActionGroup(new ActionGroup("ACL",
            new ViewAclAction()));
      }
   }

   class ViewAclAction : ConnectedAccountAction
   {
      public override string Name => "View Access Control List";

      public override Symbol Icon => Symbol.Permissions;

      public override bool CanExecute(IBlobStorage storage, Blob blob) => true;

      public override async Task ExecuteAsync(IBlobStorage storage, Blob blob, object arg)
      {
         IAzureDataLakeStorage lake = (IAzureDataLakeStorage)storage;

         await ServiceLocator.AppDialogs.ShowGen2AclEditorAsync(lake, blob);
      }
   }

}
