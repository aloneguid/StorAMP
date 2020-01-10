using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Storage.Net;
using Storage.Net.Blobs;
using NetBox.Async;
using System.Buffers;
using Humanizer;

namespace StorAmp.Core.Tasks
{
   public class CopyFilesTask : BackgroundTask
   {
      private string _sourceRoot;
      private long _totalSizeBytes;
      private long _bytesComplete;
      private int _filesComplete;
      private readonly List<Blob> _allSourceBlobs = new List<Blob>();
      private readonly List<Blob> _failedBlobs = new List<Blob>();
      private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

      private readonly IBlobStorage _sourceStorage;
      private readonly IReadOnlyCollection<Blob> _sourceBlobs;
      private readonly IBlobStorage _destinationStorage;
      private readonly string _destinationRootPath;

      public CopyFilesTask(
         IBlobStorage sourceStorage, IReadOnlyCollection<Blob> sourceBlobs,
         IBlobStorage destinationStorage, string destinationRootPath) : base("copy")
      {
         Message = "Initialising...";
         _sourceStorage = sourceStorage;
         _sourceBlobs = sourceBlobs;
         _destinationStorage = destinationStorage;
         _destinationRootPath = destinationRootPath;
      }

      public override async Task ExecuteAsync()
      {
         IsIndeterminate = true;
         await DiscoverBlobsWorkflowAsync(_sourceBlobs);

         IsIndeterminate = false;
         await CopyBlobsAsync(_allSourceBlobs);
      }

      private async Task DiscoverBlobsWorkflowAsync(IReadOnlyCollection<Blob> sourceBlobs)
      {
         foreach(Blob blob in sourceBlobs)
         {
            if(_sourceRoot == null)
            {
               _sourceRoot = StoragePath.Normalize(blob.FolderPath);
               if(StoragePath.IsRootPath(_sourceRoot))
                  _sourceRoot = string.Empty;
            }

            await DiscoverBlobAsync(blob);
         }
      }

      private async Task DiscoverBlobAsync(Blob blob)
      {
         if(blob.Kind == BlobItemKind.File)
         {
            if(blob.Size != null)
               _totalSizeBytes += blob.Size.Value;

            _allSourceBlobs.Add(blob);
         }
         else
         {
            Message = $"analysing {blob.FullPath}...";

            IReadOnlyCollection<Blob> children = await _sourceStorage.ListAsync(folderPath: blob.FullPath, recurse: false, includeAttributes: true);

            foreach(Blob child in children)
            {
               await DiscoverBlobAsync(child);
            }
         }
      }

      private async Task CopyBlobsAsync(IReadOnlyCollection<Blob> files)
      {
         using(var limiter = new AsyncLimiter(GlobalSettings.Default.MaxParallelUploads))
         {
            await Task.WhenAll(files.Where(f => f.Kind == BlobItemKind.File).Select(b => CopyBlobAsync(b, limiter)));
         }
      }

      private async Task CopyBlobAsync(Blob blob, AsyncLimiter limiter)
      {
         string destPath = blob.FullPath.Substring(_sourceRoot.Length);
         destPath = StoragePath.Combine(_destinationRootPath, destPath);

         try
         {
            using(await limiter.AcquireOneAsync())
            {
               using(Stream src = await _sourceStorage.OpenReadAsync(blob))
               {
                  if(src != null)
                  {
                     await _destinationStorage.WriteAsync(destPath, src);

                     _bytesComplete += blob.Size.Value;
                     UpdateProgress(_bytesComplete, _totalSizeBytes);
                  }
               }
            }
         }
         catch(Exception ex)
         {
            _failedBlobs.Add(blob);
            Log.Error(ex, "filed to copy");
         }

         _filesComplete += 1;
         Message = $"{_filesComplete} of {"blob".ToQuantity(_allSourceBlobs.Count)} copied";
      }
   }
}
