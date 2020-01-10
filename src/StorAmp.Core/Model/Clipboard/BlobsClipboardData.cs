using System;
using System.Collections.Generic;
using System.Linq;
using Storage.Net.Blobs;

namespace StorAmp.Core.Model.Clipboard
{
   public class BlobsClipboardData
   {
      public BlobsClipboardData(ConnectedAccount account, IBlobStorage storage, IEnumerable<Blob> blobs)
      {
         Account = account ?? throw new ArgumentNullException(nameof(account));
         Storage = storage ?? throw new ArgumentNullException(nameof(storage));
         if(blobs is null)
            throw new ArgumentNullException(nameof(blobs));
         Blobs = blobs.ToList();
      }

      public ConnectedAccount Account { get; }

      public IBlobStorage Storage { get; }

      public IReadOnlyCollection<Blob> Blobs { get; }
   }
}
