using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using NetBox.Extensions;
using Serilog;
using Storage.Net;
using StorAmp.Core.Model;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Azure
{
   class AzureStorageAccount : HierarchicalResource
   {
      private readonly IStorageAccount _storageAccount;
      private string _primarySharedKey;
      private string _secondarySharedKey;

      public AzureStorageAccount(IStorageAccount storageAccount)
         : base(storageAccount.Name,
              storageAccount.HnsEnabled.GetValueOrDefault() ? "account/azure.datalake.gen2" : "account/azure.blob")
      {
         _storageAccount = storageAccount;
         IsAutoexpandable = false;

         Children.Add(new AzureBlobStorage(storageAccount));
         Children.Add(new AzureFileShare(storageAccount));
         //Children.Add(new AzureTableStorage(storageAccount));

         GetSharedKeyAsync().Forget();

         AddCommandGroup(new HierarchicalResourceCommand("properties", Symbol.Setting, ShowPropertiesAsync));
      }

      private async Task GetSharedKeyAsync()
      {
         IReadOnlyList<StorageAccountKey> keys = await _storageAccount.GetKeysAsync();
         if(keys?.Count == 0)
            return;
         _primarySharedKey = keys[0].Value;
         _secondarySharedKey = keys[1].Value;

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Copy primary key", Symbol.Copy, () =>
            {
               ServiceLocator.SystemService.SetClipboardText(_primarySharedKey);
               return Task.CompletedTask;
            }),
            new HierarchicalResourceCommand("Copy secondary key", Symbol.Copy, () =>
            {
               ServiceLocator.SystemService.SetClipboardText(_secondarySharedKey);
               return Task.CompletedTask;
            }
            )));

         foreach(HierarchicalResource child in Children)
         {
            if(child is AzureStorageSubAccount sa)
            {
               sa.PrimarySharedKey = _primarySharedKey;
               sa.SecondarySharedKey = _secondarySharedKey;
            }
         }
      }

      private async Task ShowPropertiesAsync()
      {
         EventLog.LogEvent("showAzureStorageAccountProperties", "name: {name}", _storageAccount.Name);

         var props = new Dictionary<string, object>
         {
            ["Name"] = _storageAccount.Name,
            ["Created"] = _storageAccount.CreationTime,
            ["Access Tier"] = _storageAccount.AccessTier,
            ["Custom Domain"] = _storageAccount.CustomDomain?.Name,
            ["Id"] = _storageAccount.Id,
            ["Kind"] = _storageAccount.Kind,
            ["Last Geo Failover"] = _storageAccount.LastGeoFailoverTime,
            ["Region"] = _storageAccount.RegionName,
            ["Resource Group"] = _storageAccount.ResourceGroupName,
            ["Sku"] = _storageAccount.SkuType.Name,
            ["System Assigned MSI Principal ID"] = _storageAccount.SystemAssignedManagedServiceIdentityPrincipalId,
            ["System Assigned MSI Tenant ID"] = _storageAccount.SystemAssignedManagedServiceIdentityTenantId,
            ["Type"] = _storageAccount.Type,
            ["Can Access from Azure"] = _storageAccount.CanAccessFromAzureServices
         };

         if(_primarySharedKey != null)
         {
            props["Key (Primary)"] = _primarySharedKey;
         }
         if(_secondarySharedKey != null)
         {
            props["Key (Secondary)"] = _secondarySharedKey;
         }

         await Dialogs.ShowPropertiesAsync("Properties", props);
      }
   }
}
