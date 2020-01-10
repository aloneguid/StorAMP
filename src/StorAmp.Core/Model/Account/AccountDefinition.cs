using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Storage.Net.ConnectionString;
using StorAmp.Core.Services;
using StorAmp.Core.ViewModel.Blobs.Definitions;
using StorAmp.Core.ViewModel.Msg.Definitions;

namespace StorAmp.Core.Model.Account
{
   public abstract class AccountDefinition
   {
      private static readonly Dictionary<string, AccountDefinition> _prefixToAccount = new Dictionary<string, AccountDefinition>();

      static AccountDefinition()
      {
         Register(new LocalDiskAccount());
         Register(new AzureBlobStorageAccount());
         Register(new AzureFilesAccount());
         Register(new AmazonS3StorageAccount());
         Register(new AzureKeyVaultAccount());
         Register(new InMemoryAccount());
         Register(new AzdbfsAccount());
         Register(new AzureDataLakeGen1Account());
         Register(new AzureDataLakeGen2Account());
         Register(new GoogleStorageAccount());

#if DEBUG
         Register(new AzureEventHubStorageAccountDefinition());
         Register(new AzureStorageQueuesAccountDefinition());
#endif
      }

      private static void Register(AccountDefinition worker)
      {
         _prefixToAccount[worker.Prefix] = worker;
         AllDefinitions.Add(worker);
      }

      public static ObservableCollection<AccountDefinition> AllDefinitions { get; } = new ObservableCollection<AccountDefinition>();

      protected AccountDefinition(
         string prefix,
         string displayName,
         params AccountField[] fields)
      {
         Type = "blob";
         Prefix = prefix;
         DisplayName = displayName;
         ConnectionTypes = new List<AccountConnectionType>
         {
            new AccountConnectionType("default", "default", fields)
         };
      }

      protected AccountDefinition(
         string type,
         string prefix,
         string displayName,
         params AccountConnectionType[] connectionTypes)
      {
         Type = type;
         Prefix = prefix;
         DisplayName = displayName;
         ConnectionTypes = connectionTypes.ToList();
      }

      public string Prefix { get; }
      public string DisplayName { get; }

      public IReadOnlyCollection<AccountConnectionType> ConnectionTypes { get; set; }

      public IReadOnlyCollection<AccountField> Fields => ConnectionTypes.First().Fields;

      public string Type { get; set; }

      public async Task<Exception> ValidateInteractiveAsync(string accountId, StorageConnectionString connectionString)
      {
         IDialogService dlg = ServiceLocator.GetInstance<IDialogService>();

         try
         {
            Exception ex = await TestAndGetErrorAsync(accountId, connectionString);

            if(ex == null)
            {
               await dlg.ShowMessageAsync("All Good", "The connection is valid.");
            }
            else
            {
               await dlg.ShowMessageAsync("Validation error", "Cannot connect: " + ex.Message + ".");
            }

            return ex;
         }
         catch(NotImplementedException)
         {
            await dlg.ShowMessageAsync("Not Supported", "Validation on this account type is not supported.");
         }


         return null;
      }

      public virtual async Task<Exception> TestAndGetErrorAsync(string accountId, StorageConnectionString connectionString)
      {
         throw new NotImplementedException();
      }

      public static AccountDefinition GetByPrefix(string prefix)
      {
         if(prefix == null || !_prefixToAccount.TryGetValue(prefix, out AccountDefinition worker))
            return null;

         return worker;
      }
   }

}
