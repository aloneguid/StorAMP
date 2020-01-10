using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace StorAmp.Core.Model
{
   public class ApplicationConfiguration
   {
      [JsonProperty("attached")]
      public ConnectedEntity RootEntity { get; set; } = new ConnectedFolder();

      [JsonProperty("azure")]
      public ObservableCollection<ConnectedAzureAccount> AzureAccounts { get; set; } = new ObservableCollection<ConnectedAzureAccount>();
   }
}
