using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Storage.Net.Blobs;
using StorAmp.Core.Model.Messages;

namespace StorAmp.Core.Tasks
{
   public class RenameFilesTask : BackgroundTask
   {
      private readonly IBlobStorage _blobStorage;
      private readonly Blob _blob;
      private readonly string _newPath;

      public RenameFilesTask(IBlobStorage blobStorage, Blob blob, string newPath) : base("rename")
      {
         _blobStorage = blobStorage;
         _blob = blob;
         _newPath = newPath;
      }

      public override async Task ExecuteAsync()
      {
         IsIndeterminate = true;
         Message = "renaming...";
         Abstract = $"{_blob.Name}";

         await _blobStorage.RenameAsync(_blob.FullPath, _newPath);

         var message = new FolderUpdatedMessage(_blobStorage, _blob.FolderPath);
         message.ReplacedBlobs[_blob] = new Blob(_newPath, _blob.Kind);
         Messenger.Default.Send(message);
      }
   }
}
