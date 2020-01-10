using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Management.Storage.Fluent;
using Storage.Net;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Azure
{
   public class AzureFileShare : AzureStorageSubAccount
   {
      public AzureFileShare(IStorageAccount storageAccount) : base(storageAccount, "File Shares", "account/azure.file")
      {
      }

      protected override ConnectedAccount CreateConnectedAccount()
      {
         string cs = StorageFactory.ConnectionStrings.ForAzureFilesStorageWithSharedKey(StorageAccount.Name, PrimarySharedKey).ToString();
         return new ConnectedAccount(cs) { DisplayName = StorageAccount.Name };
      }
   }
}
