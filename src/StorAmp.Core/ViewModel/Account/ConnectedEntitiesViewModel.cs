using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Serilog;
using StorAmp.Core.Extensions;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Account
{
   public partial class ConnectedEntitiesViewModel : HierarchicalResource
   {
      public ConnectedEntitiesViewModel() : base("Accounts", "paperclip")
      {
         IsExpanded = true;

         Children = ToHR(ConnectedAccount.RootFolder.Children);

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Add account", Symbol.Account, () => AppDialogs.ShowConnectedAccountDialogAsync(null, null)),
            new HierarchicalResourceCommand("Add folder", Symbol.NewFolder, AddRootFolderAsync)));
      }

      private static ObservableCollection<HierarchicalResource> ToHR(ObservableCollection<ConnectedEntity> connectedEntities)
      {
         return new SynchronisedObservableCollection<HierarchicalResource, ConnectedEntity>(
            connectedEntities,
            c => ToHR(c));
      }

      private static HierarchicalResource ToHR(ConnectedEntity connectedEntity)
      {
         if(connectedEntity is ConnectedAccount ca)
         {
            return new ConnectedAccountViewModel(ca);
         }

         if(connectedEntity is ConnectedFolder cf)
         {
            var hr = new ConnectedFolderViewModel(cf);
            hr.Children = new SynchronisedObservableCollection<HierarchicalResource, ConnectedEntity>(
               cf.Children,
               c => ToHR(c));

            return hr;
         }

         return null;
      }

      private async Task AddRootFolderAsync()
      {
         string name = await Dialogs.AskStringInputAsync("Add folder", "Name");
         if(name == null) return;

         EventLog.LogEvent("addRootFolder", "name: {name}", name);

         ConnectedAccount.RootFolder.Children.Add(new ConnectedFolder { DisplayName = name });
         ConnectedAccount.Save();
      }
   }
}
