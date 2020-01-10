using System.Collections.Generic;
using Storage.Net.Blobs;

namespace StorAmp.Core.Model.Messages
{
   public class FolderUpdatedMessage
   {
      public FolderUpdatedMessage(IBlobStorage blobStorage, string folderPath)
      {
         BlobStorage = blobStorage;
         FolderPath = folderPath;
      }

      public IBlobStorage BlobStorage { get; }
      public string FolderPath { get; }

      public Dictionary<Blob, Blob> ReplacedBlobs = new Dictionary<Blob, Blob>();
   }
}
