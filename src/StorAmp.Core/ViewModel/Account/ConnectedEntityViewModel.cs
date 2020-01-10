using System.ComponentModel;
using System.Threading.Tasks;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Account
{
   public class ConnectedEntityViewModel : HierarchicalResource
   {
      private readonly ConnectedEntity _connectedEntity;

      public ConnectedEntityViewModel(ConnectedEntity connectedEntity, string iconPath) : base(connectedEntity.DisplayName, iconPath)
      {
         IsExpanded = true;

         CommandGroups.Add(new HierarchicalResourceCommandGroup(
            new HierarchicalResourceCommand("delete", Symbol.Delete, DeleteAsync),
            new HierarchicalResourceCommand("rename", Symbol.Rename, RenameAsync)
            ));

         connectedEntity.PropertyChanged += _ca_PropertyChanged;
         _connectedEntity = connectedEntity;
      }

      private void _ca_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if(e.PropertyName == nameof(ConnectedEntity.DisplayName))
         {
            DisplayName = _connectedEntity.DisplayName;
         }
      }


      private async Task RenameAsync()
      {
         string newName = await Dialogs.AskStringInputAsync("Rename", "Name", DisplayName);

         if(!string.IsNullOrEmpty(newName))
         {
            _connectedEntity.DisplayName = newName;
            ConnectedAccount.Save();
         }
      }

      private async Task DeleteAsync()
      {
         if(await Dialogs.AskYesNoAsync("Delete Account", $"Are you sure you want to delete '{DisplayName}'? This cannot be undone."))
         {
            ConnectedAccount.RootFolder.DeleteById(_connectedEntity.Id);
            ConnectedAccount.Save();
         }
      }
   }
}
