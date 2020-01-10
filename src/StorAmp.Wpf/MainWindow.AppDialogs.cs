using System.Threading.Tasks;
using MahApps.Metro.SimpleChildWindow;
using Storage.Net.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs;
using StorAmp.Core.Model;
using StorAmp.Core.Services;
using StorAmp.Core.ViewModel.Azure;
using StorAmp.Core.ViewModel.Blobs;
using StorAmp.Wpf.Wpf;
using StorAmp.Wpf.Wpf.Azure;
using StorAmp.Wpf.Wpf.Blobs;
using StorAmp.Wpf.Wpf.Blobs.Impl.AzureBlobStorage;

namespace CloudExplorer.Wpf
{
   public partial class MainWindow : IAppDialogsService
   {
      public async Task ShowAzureAccountDialogAsync()
      {
         var vm = new AzureAccountControlViewModel();

         await this.ShowDialogAsync("Azure Account",
            new AzureAccountControl { DataContext = vm },
            "Add",
            vm.CompleteAuthCommand);
      }

      public async Task ShowAzureBlobStorageGetAccountSasAsync()
      {
         await this.ShowDialogAsync("Generate SAS", new GetSharedAccessSignatureControl(), "Create", null);
      }

      public async Task ShowConnectedAccountDialogAsync(ConnectedFolder cf, ConnectedAccount ca)
      {
         await this.ShowChildWindowAsync(new AddAccountChildWindow(cf, ca) { IsModal = true });
      }

      public async Task ShowGen2AclEditorAsync(IAzureDataLakeStorage azureDataLakeStorage, Blob blob)
      {
         var vm = new AdlsGen2PermissionsViewModel(azureDataLakeStorage, blob);

         await this.ShowDialogAsync(
            "Access Control List",
            new AclEditor { DataContext = vm },
            "Update",
            null);
      }
   }
}
