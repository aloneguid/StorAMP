using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NetBox.Extensions;
using Storage.Net.Blobs;

namespace StorAmp.Core.Tasks
{
   public class DownloadAndViewLocallyTask : BackgroundTask
   {
      private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;
      private readonly IBlobStorage _storage;
      private readonly Blob _blob;

      public DownloadAndViewLocallyTask(IBlobStorage storage, Blob blob) : base("download")
      {
         Abstract = $"{blob.Name}";
         _storage = storage;
         _blob = blob;
      }

      public override async Task ExecuteAsync()
      {
         string localPath = Path.Combine(Path.GetTempPath(), _blob.Name);

         if(_blob.Size == null)
         {
            IsIndeterminate = true;
            await _storage.ReadToFileAsync(_blob, localPath);
         }
         else
         {
            IsIndeterminate = false;
            int totalRead = 0;
            byte[] buffer = _pool.Rent(4096);

            try
            {
               using(Stream src = await _storage.OpenReadAsync(_blob))
               {
                  using(Stream tgt = File.Create(localPath))
                  {
                     int read;

                     while((read = (await src.ReadAsync(buffer, 0, buffer.Length))) > 0)
                     {
                        totalRead += read;


                        await tgt.WriteAsync(buffer, 0, read);

                        UpdateProgress(totalRead, _blob.Size.Value);
                        Message = $"{totalRead.ToFileSizeUiString()}";
                     }
                  }
               }
            }
            finally
            {
               _pool.Return(buffer);
            }
         }

         new Process
         {
            StartInfo = new ProcessStartInfo(localPath)
            {
               UseShellExecute = true
            }
         }.Start();
      }
   }
}
