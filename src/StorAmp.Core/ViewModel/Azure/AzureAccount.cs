using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using NetBox.Extensions;
using Serilog;
using StorAmp.Core.Model;
using StorAmp.Core.Services.Az;

namespace StorAmp.Core.ViewModel.Azure
{
   class AzureAccount : HierarchicalResource
   {
      private readonly MultiTenantAzure _client;
      private readonly ConnectedAzureAccount _connectedAzureAccount;

      public AzureAccount(ConnectedAzureAccount connectedAzureAccount) : base(connectedAzureAccount.DisplayName, "azure/directory")
      {
         _client = MultiTenantAzure.FromAccount(connectedAzureAccount);

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("refresh", Symbol.Refresh, RefreshAsync),
            new HierarchicalResourceCommand("delete", Symbol.Delete, DeleteAsync)));

         RefreshAsync().Forget();
         _connectedAzureAccount = connectedAzureAccount;
      }

      private async Task RefreshAsync()
      {
         EventLog.LogEvent("refreshAzureAccount", "name: {name}", _client?.UserInfo?.DisplayableId);

         IsLoading = true;

         try
         {
            Children.Clear();

            IReadOnlyCollection<KeyValuePair<string, ISubscription>> allSubscriptions = await _client.ListAllSubscriptionsAsync();

            foreach(KeyValuePair<string, ISubscription> sub in allSubscriptions)
            {
               Children.Add(new AzureSubscription(_client.GetAuthenticated(sub.Key), sub.Value));
            }
         }
         catch(Exception ex)
         {
            Error = ex;
         }
         finally
         {
            IsLoading = false;
         }
      }

      private async Task DeleteAsync()
      {
         EventLog.LogEvent("deleteAzureAccount", "name: {name}", _client?.UserInfo?.DisplayableId);

         ConnectedAccount.AzureAccounts.Remove(_connectedAzureAccount);
         ConnectedAccount.Save();
      }
   }
}
