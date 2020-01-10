using Storage.Net;
using StorAmp.Core.Model.Account;

namespace StorAmp.Core.ViewModel.Msg.Definitions
{
   class AzureStorageQueuesAccountDefinition : AccountDefinition
   {
      public AzureStorageQueuesAccountDefinition() : base(
         "msg",
         KnownPrefix.AzureQueueStorage,
         "Azure Storage Queues",
         new AccountConnectionType("sk", "Key",
            new AccountField("account", "Name"),
            new AccountField("key", "Key")))
      {
      }
   }
}
