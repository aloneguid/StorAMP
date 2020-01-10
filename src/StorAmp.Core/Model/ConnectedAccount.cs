using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Storage.Net.ConnectionString;
using StorAmp.Core.Model.Account;
using StorAmp.Core.Extensions;
using Azure.Core;
using System.Linq;
using Storage.Net.Blobs;
using Storage.Net;
using System;

namespace StorAmp.Core.Model
{

   public class ConnectedAccount : ConnectedEntity
   {
      public const string AzureAccountIdParameterName = "azureAccountId";
      private static bool _configLoaded;
      private static ApplicationConfiguration _globalConfig;

      public ConnectedAccount(string connectionString)
      {
         StorageConnectionString = new StorageConnectionString(connectionString);

         Definition = AccountDefinition.GetByPrefix(StorageConnectionString.Prefix);
      }

      [JsonProperty("system")]
      public string System { get; set; } = "blob";

      [JsonProperty("connectionType")]
      public string ConnectionTypeToken { get; set; }

      [JsonIgnore]
      public string Prefix => StorageConnectionString?.Prefix;

      [JsonIgnore]
      public StorageConnectionString StorageConnectionString { get; set; }

      [JsonProperty("connectionString")]
      public string ConnectionString
      {
         get => StorageConnectionString?.ToString();
         set
         {
            if(value == null)
            {
               StorageConnectionString = null;
            }
            else
            {
               StorageConnectionString = new StorageConnectionString(value);
            }
         }
      }

      public override string ToString() => $"{DisplayName}";

      [JsonIgnore]
      public AccountDefinition Definition { get; }

      private static ApplicationConfiguration GlobalConfig
      {
         get
         {
            if(!_configLoaded)
            {
               _globalConfig = LoadGlobalConfig();
               Save();
               _configLoaded = true;
            }

            return _globalConfig;
         }
      }

      public static ConnectedFolder RootFolder => (ConnectedFolder)GlobalConfig.RootEntity;

      public static ObservableCollection<ConnectedAzureAccount> AzureAccounts => GlobalConfig.AzureAccounts;

      public IBlobStorage CreateBlobStorage()
      {
         if(StorageConnectionString.Parameters.TryGetValue(AzureAccountIdParameterName, out string azureAccountId))
         {
            TokenCredential tokenCredential = GetAzureTokenCredential(azureAccountId);

            if(StorageConnectionString.Prefix == KnownPrefix.AzureBlobStorage)
            {
               return StorageFactory.Blobs.AzureBlobStorageWithTokenCredential(
                  StorageConnectionString[KnownParameter.AccountName],
                  tokenCredential);
            }

            throw new NotSupportedException();
         }

         return StorageFactory.Blobs.FromConnectionString(ConnectionString);
      }

      public static TokenCredential GetAzureTokenCredential(string accountId)
      {
         ConnectedAzureAccount caza = AzureAccounts.FirstOrDefault(a => a.Id == accountId);
         if(caza == null)
            return null;
         return caza.AsTokenCredential();
      }

      private static ApplicationConfiguration LoadGlobalConfig()
      {
         ApplicationConfiguration config = StorAmpJson.ReadFromFile<ApplicationConfiguration>(GlobalSettings.GetAccountsJsonPath());

         if(config?.RootEntity.Children?.Count > 0)
         {
            return config;
         }
         else
         {
            config = new ApplicationConfiguration
            {
               RootEntity = new ConnectedFolder
               {
                  Children = new ObservableCollection<ConnectedEntity>
                  {
                     new ConnectedAccount("disk://path=c:\\")
                     {
                        DisplayName = "Drive C:",
                     }
                  }
               }
            };
         }

         return config;
      }

      public static void Save()
      {
         StorAmpJson.WriteToFile(_globalConfig, GlobalSettings.GetAccountsJsonPath());
      }

      public static void AddToLastFolderAndSave(ConnectedAccount connectedAccount)
      {
         GlobalState.LastConnectedFolder.Children.Add(connectedAccount);
         Save();
      }
   }
}
