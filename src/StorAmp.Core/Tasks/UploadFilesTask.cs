using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using NetBox.Async;
using NetBox.Extensions;
using Storage.Net;
using Storage.Net.Blobs;
using StorAmp.Core.Model.Messages;

namespace StorAmp.Core.Tasks
{
   public class UploadFilesTask : BackgroundTask
   {
      private readonly IBlobStorage _storage;
      private readonly string _targetRootPath;
      private readonly IReadOnlyCollection<string> _localPaths;
      private readonly string _rootSourceFolder;
      private readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

      //progress tracking
      private readonly List<string> _allFiles = new List<string>();
      private long _totalSize;
      private long _copied;
      private int _filesCopied;
      private readonly Dictionary<string, Exception> _failedBlobs = new Dictionary<string, Exception>();

      public UploadFilesTask(IBlobStorage storage, string targetRootPath, IReadOnlyCollection<string> localPaths) : base("upload")
      {
         _storage = storage;
         _targetRootPath = targetRootPath;
         _localPaths = localPaths;
         _rootSourceFolder = GetRootFolder(localPaths);
      }

      private static string GetRootFolder(IEnumerable<string> files)
      {
         return files
            .OrderBy(f => f.Length)
            .Select(f => new FileInfo(f).DirectoryName)
            .First();
      }

      public override async Task ExecuteAsync()
      {
         IsIndeterminate = true;
         Message = "scanning file list";
         foreach(string path in _localPaths)
         {
            AnalysePath(path);
         }

         IsIndeterminate = false;
         using(var limiter = new AsyncLimiter(GlobalSettings.Default.MaxParallelUploads))
         {
            await Task.WhenAll(_allFiles.Select(path => UploadFile(limiter, path)));
         }

         SendUpdate();

         Messenger.Default.Send(new FolderUpdatedMessage(_storage, _targetRootPath));

         //Message = $"uploaded {"file".ToQuantity(_filesCopied)} and {_totalSize.ToFileSizeUiString()}";
      }

      private async Task UploadFile(AsyncLimiter limiter, string localPath)
      {
         using(await limiter.AcquireOneAsync())
         {
            using(Stream source = File.OpenRead(localPath))
            {
               try
               {
                  string localRelativePath = localPath.Substring(_rootSourceFolder.Length);
                  string destPath = StoragePath.Combine(_targetRootPath, localRelativePath.Replace(Path.DirectorySeparatorChar, StoragePath.PathSeparator));

                  await _storage.WriteAsync(destPath, source);
                  _copied += new FileInfo(localPath).Length;
                  SendUpdate();
               }
               catch(Exception ex)
               {
                  _failedBlobs[localPath] = ex;
               }
            }

            _filesCopied += 1;
         }

         SendUpdate();
      }

      private void SendUpdate()
      {
         var sb = new StringBuilder();
         sb.Append($"{_copied.ToFileSizeUiString()}/{_totalSize.ToFileSizeUiString()} uploaded ({_filesCopied}/{_allFiles.Count})");
         if(_failedBlobs.Count > 0)
         {
            sb.Append(", ");
            sb.Append(_failedBlobs.Count);
            sb.Append(" failed.");
         }

         Message = sb.ToString();
         UpdateProgress(_copied, _totalSize);
      }

      private void AnalysePath(string path)
      {
         if((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
         {
            foreach(string ipath in Directory.GetDirectories(path))
            {
               AnalysePath(ipath);
            }

            foreach(string ipath in Directory.GetFiles(path))
            {
               AnalyseFile(ipath);
            }
         }
         else
         {
            AnalyseFile(path);
         }
      }

      private void AnalyseFile(string filePath)
      {
         _allFiles.Add(filePath);
         _totalSize += new FileInfo(filePath).Length;
      }
   }
}
