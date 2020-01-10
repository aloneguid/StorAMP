using System.Threading.Tasks;
using Serilog;
using StorAmp.Core.Extensions;
using StorAmp.Core.Model;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Azure
{
   class MicrosoftAzure : HierarchicalResource
   {
      public MicrosoftAzure() : base("Microsoft Azure", "azure/logo")
      {
         IsExpanded = true;

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("add new account", Symbol.Add, AddAzureAccountAsync)));

         Children = new SynchronisedObservableCollection<HierarchicalResource, ConnectedAzureAccount>(
            ConnectedAccount.AzureAccounts,
            caa => new AzureAccount(caa));
      }

      private async Task AddAzureAccountAsync()
      {
         EventLog.LogEvent("addAzureAccount", "fromMenu");

         await ServiceLocator.AppDialogs.ShowAzureAccountDialogAsync();
      }
   }
}
