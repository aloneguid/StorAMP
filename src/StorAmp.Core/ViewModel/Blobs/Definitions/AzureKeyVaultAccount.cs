using Storage.Net;
using Storage.Net.Blobs;
using StorAmp.Core.Model.Account;

namespace StorAmp.Core.ViewModel.Blobs.Definitions
{
   class AzureKeyVaultAccount : BlobApplicationAccount
   {
      public AzureKeyVaultAccount() : base(
         "azure.keyvault",
         "Azure Key Vault",
         new AccountField("vaultUri", "Key Vault URI"),
         new AccountField(KnownParameter.TenantId, "Tenant ID"),
         new AccountField(KnownParameter.ClientId, "Client ID"),
         new AccountField(KnownParameter.ClientSecret, "Client Secret"))
      {
         AddColumnView(new ExtraView());
      }

      public class ExtraView : ExtraColumnView
      {
         public ExtraView() : base("any",
            new ExtraPropertyColumn("Created", "CreatedOn"),
            new ExtraPropertyColumn("Enabled", "IsEnabled"),
            new ExtraPropertyColumn("Expires", "ExpiresOn"),
            new ExtraPropertyColumn("Created", "CreatedOn"),
            new ExtraPropertyColumn("Managed", "IsManaged"),
            new ExtraPropertyColumn("Recovery Level", "RecoveryLevel"),
            new ExtraPropertyColumn("Version", "Version"))
         {
         }

         public override bool IsMatch(Blob anyBlob) => true;
      }
   }

}
