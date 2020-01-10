using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StorAmp.Core.Model;
using NetBox.Extensions;
using Storage.Net.Blobs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using EventLog = Serilog.EventLog;

namespace StorAmp.Core.ViewModel
{
   public partial class ViewEditControlViewModel : ViewModelBase
   {
      public event Action<bool> OnDone;
      public event Action<Blob, string, string> OnView;
      public event Action<bool> OnClearView;
      public event Action<bool> OnSave;
      private IBlobStorage _storage;

      public ViewEditControlViewModel()
      {
         TempFilePath = Path.GetTempFileName();

         LoadViewers();
      }

      public async Task PreviewAsync(IBlobStorage storage, Blob blob, string forcedExtension = null)
      {
         EventLog.LogEvent("viewFile", "{name} {path} {forcedExtension}", blob.Name, blob.FullPath, forcedExtension);

         TempFilePath = Path.Combine(Path.GetTempPath(), "StorAmp");
         if(!Directory.Exists(TempFilePath))
            Directory.CreateDirectory(TempFilePath);
         string ext = forcedExtension ?? Path.GetExtension(blob);
         TempFilePath = Path.Combine(TempFilePath, Path.GetRandomFileName() + ext);
         _storage = storage;
         SelectedBlob = null;
         SelectedViewer = null;
         IsLoading = true;
         ErrorText = null;
         try
         {
            await DownloadBlobAsync(blob);

            FileViewerDetails viewer = Viewers.FirstOrDefault(v => v.Extensions.Contains(ext));
            SelectedBlob = blob;
            SelectedViewer = viewer;
            HasChanged = false;
         }
         catch(Exception ex)
         {
            ErrorText = ex.Message;
            Log.Error(ex, "failed to preview {blob}", blob.Name);
         }
         finally
         {
            IsLoading = false;
         }
      }

      private async Task SaveAsync()
      {
         IsLoading = true;

         try
         {
            OnSave?.Invoke(true);

            using(Stream source = File.OpenRead(TempFilePath))
            {
               await _storage.WriteAsync(SelectedBlob, source);
            }

            OnDone?.Invoke(true);
         }
         finally
         {
            IsLoading = false;
         }

      }

      private async Task DownloadBlobAsync(Blob blob)
      {
         using(Stream source = await _storage.OpenReadAsync(blob))
         {
            using(Stream dest = File.Create(TempFilePath))
            {
               await source.CopyToAsync(dest);
            }
         }
      }

      private void LoadViewers()
      {
         var viewers = new List<FileViewerDetails>();

         foreach(string formatLine in Strings.TextEditor_SupportedFormats.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
         {
            string[] kv = formatLine.Split(new[] { '|' }, 2);
            viewers.Add(
               new FileViewerDetails
               {
                  DisplayName = kv[0],
                  Extensions = new HashSet<string>(kv[1].Split(','), StringComparer.OrdinalIgnoreCase)
               });
         }

         Viewers.AddRange(viewers.OrderBy(v => v.DisplayName));
      }

      #region [ Commands ]


      private RelayCommand _goBackCommand;

      /// <summary>
      /// Gets the GoBackCommand.
      /// </summary>
      public RelayCommand GoBackCommand
      {
         get
         {
            return _goBackCommand
                ?? (_goBackCommand = new RelayCommand(
                () =>
                {
                   OnDone?.Invoke(true);
                }));
         }
      }

      private RelayCommand _saveCommand;

      /// <summary>
      /// Gets the SaveCommand.
      /// </summary>
      public RelayCommand SaveCommand
      {
         get
         {
            return _saveCommand
                ?? (_saveCommand = new RelayCommand(
                () =>
                {
                   SaveAsync().Forget();
                }, () => HasChanged));
         }
      }

      #endregion
   }
}
