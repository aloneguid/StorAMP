using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using NetBox.Extensions;
using Storage.Net.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs;
using Storage.Net.Microsoft.Azure.Storage.Blobs.Gen2.Model;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Acl;

namespace StorAmp.Core.ViewModel.Blobs
{
   public class AdlsGen2PermissionsViewModel : ViewModelBase
   {
      private readonly IAzureDataLakeStorage _azureDataLakeStorage;

      public AdlsGen2PermissionsViewModel()
      {

      }

      public AdlsGen2PermissionsViewModel(IAzureDataLakeStorage azureDataLakeStorage, Blob blob)
      {
         _azureDataLakeStorage = azureDataLakeStorage;
         Blob = blob;
      }

      private async Task InitialiseAsync()
      {
         AccessControl acl = await _azureDataLakeStorage.GetAccessControlAsync(Blob, true);

         Acl = new EditableAcl(acl);
      }

      #region [ Properties ]


      private EditableAcl _acl;
      public EditableAcl Acl
      {
         get => _acl;
         set { Set(() => Acl, ref _acl, value); }
      }


      private Blob _Blob;
      public Blob Blob
      {
         get => _Blob;
         set { Set(() => Blob, ref _Blob, value); InitialiseAsync().Forget();  }
      }

      #endregion
   }
}
