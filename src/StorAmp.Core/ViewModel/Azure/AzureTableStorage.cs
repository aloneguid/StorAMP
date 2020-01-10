using System;
using Microsoft.Azure.Management.Storage.Fluent;
using Storage.Net;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Azure
{
   public class AzureTableStorage : AzureStorageSubAccount
   {
      public AzureTableStorage(IStorageAccount storageAccount) : base(storageAccount, "Tables", "account/azure.tables")
      {
      }

      protected override ConnectedAccount CreateConnectedAccount()
      {
         throw new NotImplementedException();
      }
   }
}
