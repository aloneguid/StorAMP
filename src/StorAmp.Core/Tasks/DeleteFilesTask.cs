using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using Storage.Net;
using Storage.Net.Blobs;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Messages;

namespace StorAmp.Core.Tasks
{
   public class DeleteFilesTask : BackgroundTask
   {
      private readonly ConnectedAccount _connectedAccount;
      private readonly IBlobStorage _storage;
      private readonly IReadOnlyCollection<Blob> _toDelete;

      public DeleteFilesTask(ConnectedAccount connectedAccount, IBlobStorage storage, IReadOnlyCollection<Blob> toDelete)
         : base(Strings.BackgroundTask_Delete_TypeName)
      {
         _connectedAccount = connectedAccount;
         _storage = storage;
         _toDelete = toDelete;
      }

      public override async Task ExecuteAsync()
      {
         Message = $"deleting {"item".ToQuantity(_toDelete.Count)} in '{_connectedAccount.DisplayName}'";

         await _storage.DeleteAsync(_toDelete);

         Message = $"{"item".ToQuantity(_toDelete.Count)} deleted in '{_connectedAccount.DisplayName}'";
         ProgressPercentage = 100;

         Messenger.Default.Send(new FolderUpdatedMessage(_storage, _toDelete.First().FolderPath));
      }
   }
}
