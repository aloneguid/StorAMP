using Microsoft.Azure.Management.Storage.Fluent;
using Storage.Net;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Azure
{
   public class AzureBlobStorage : AzureStorageSubAccount
   {
      public AzureBlobStorage(IStorageAccount storageAccount)
         : base(storageAccount, "Blobs", storageAccount.HnsEnabled.GetValueOrDefault() ? "account/azure.datalake.gen2" : "account/azure.blob")
      {
      }

      protected override ConnectedAccount CreateConnectedAccount()
      {
         string cs = StorageAccount.HnsEnabled.GetValueOrDefault()
            ? StorageFactory.ConnectionStrings.ForAzureDataLakeStorageWithSharedKey(StorageAccount.Name, PrimarySharedKey).ToString()
            : StorageFactory.ConnectionStrings.ForAzureBlobStorageWithSharedKey(StorageAccount.Name, PrimarySharedKey).ToString();

         return new ConnectedAccount(cs) { DisplayName = StorageAccount.Name };
      }
   }
}
