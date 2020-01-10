using System.Threading.Tasks;
using Storage.Net.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs;
using StorAmp.Core.Model;

namespace StorAmp.Core.Services
{
   public interface IAppDialogsService
   {
      Task ShowConnectedAccountDialogAsync(ConnectedFolder container, ConnectedAccount ca);

      Task ShowAzureBlobStorageGetAccountSasAsync();

      Task ShowAzureAccountDialogAsync();

      Task ShowGen2AclEditorAsync(IAzureDataLakeStorage azureDataLakeStorage, Blob blob);
   }
}
