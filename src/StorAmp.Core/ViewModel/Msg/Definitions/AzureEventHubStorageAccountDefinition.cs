using System;
using System.Threading.Tasks;
using Storage.Net;
using Storage.Net.ConnectionString;
using Storage.Net.Messaging;
using StorAmp.Core.Model.Account;

namespace StorAmp.Core.ViewModel.Msg.Definitions
{
   class AzureEventHubStorageAccountDefinition : AccountDefinition
   {
      public AzureEventHubStorageAccountDefinition()
         : base("msg", "azure.eventhub", "Azure Event Hub",
              new AccountConnectionType("native", "Connection String",
                 new AccountField("native", "Connection String")))
      {
      }

      public override async Task<Exception> TestAndGetErrorAsync(string accountId, StorageConnectionString connectionString)
      {
         IMessenger messenger = StorageFactory.Messages.MessengerFromConnectionString(connectionString.ToString());

         //todo: how to validate?

         return null;
      }
   }
}
