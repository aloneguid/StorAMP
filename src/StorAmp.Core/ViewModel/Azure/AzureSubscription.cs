using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using NetBox.Extensions;
using Serilog;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Azure
{
   using IAuthenticated = Microsoft.Azure.Management.Fluent.Azure.IAuthenticated;

   class AzureSubscription : HierarchicalResource
   {
      private readonly IAzure _azure;
      private readonly Dictionary<string, AzureResourceGroup> _resourceGroups = new Dictionary<string, AzureResourceGroup>();

      public AzureSubscription(IAuthenticated authenticated, ISubscription subscription) : base(subscription.DisplayName, "azure/subscription")
      {
         _azure = authenticated.WithSubscription(subscription.SubscriptionId);

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("refresh", Symbol.Refresh, RefreshAsync)));

         RefreshAsync().Forget();
      }

      private async Task RefreshAsync()
      {
         EventLog.LogEvent("refreshAzureSubscription", "name: {name}", DisplayName);

         IsLoading = true;
         Children.Clear();

         try
         {
            IPagedCollection<IStorageAccount> accounts = await _azure.StorageAccounts.ListAsync();

            foreach(IStorageAccount sa in accounts)
            {
               if(GlobalSettings.Default.AzureGroupResourcesByResourceGroup)
               {
                  AzureResourceGroup rg = GetOrCreateResourceGroup(sa.ResourceGroupName);

                  rg.Children.Add(new AzureStorageAccount(sa));
               }
               else
               {
                  Children.Add(new AzureStorageAccount(sa));
               }
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

      private AzureResourceGroup GetOrCreateResourceGroup(string name)
      {
         if(_resourceGroups.TryGetValue(name, out AzureResourceGroup rg))
            return rg;

         rg = new AzureResourceGroup(name);
         _resourceGroups[name] = rg;
         Children.Add(rg);
         return rg;
      }
   }
}
