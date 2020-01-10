using GalaSoft.MvvmLight;
using Storage.Net.Microsoft.Azure.Storage.Blobs.Gen2.Model;

namespace StorAmp.Core.Model.Acl
{
   public class EditableAclEntry : ViewModelBase
   {
      public EditableAclEntry(AclEntry acl)
      {
         Identity = acl.Identity;
         CanRead = acl.CanRead;
         CanWrite = acl.CanWrite;
         CanExecute = acl.CanExecute;
      }

      private string _identity;
      public string Identity
      {
         get => _identity;
         set { Set(() => Identity, ref _identity, value); }
      }


      private bool _canRead;
      public bool CanRead
      {
         get => _canRead;
         set { Set(() => CanRead, ref _canRead, value); }
      }


      private bool _canWrite;
      public bool CanWrite
      {
         get => _canWrite;
         set { Set(() => CanWrite, ref _canWrite, value); }
      }


      private bool _canExecute;
      public bool CanExecute
      {
         get => _canExecute;
         set { Set(() => CanExecute, ref _canExecute, value); }
      }
   }
}
