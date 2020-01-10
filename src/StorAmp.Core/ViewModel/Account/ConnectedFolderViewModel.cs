using System.Threading.Tasks;
using Serilog;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Account
{
   public class ConnectedFolderViewModel : ConnectedEntityViewModel
   {
      public ConnectedFolderViewModel(ConnectedFolder cf) : base(cf, "folder2")
      {
         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("Add subfolder", Symbol.NewFolder, AddSubfolderAsync),
            new HierarchicalResourceCommand("Add Account...", Symbol.Account, () => AppDialogs.ShowConnectedAccountDialogAsync(cf, null))));
         Folder = cf;
      }

      public ConnectedFolder Folder { get; private set; }

      private async Task AddSubfolderAsync()
      {
         string name = await Dialogs.AskStringInputAsync("Add folder", "Name");
         if(name == null)
            return;

         EventLog.LogEvent("addRootFolder", "name: {name}", name);

         Folder.Children.Add(new ConnectedFolder { DisplayName = name });
         ConnectedAccount.Save();
      }
   }
}