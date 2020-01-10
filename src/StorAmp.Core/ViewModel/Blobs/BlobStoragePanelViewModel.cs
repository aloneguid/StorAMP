using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using StorAmp.Core.Extensions;
using StorAmp.Core.Model;
using StorAmp.Core.Services;
using Storage.Net;
using Storage.Net.Blobs;
using StorAmp.Core.Tasks;
using Humanizer;
using StorAmp.Core.ViewModel.Blobs;
using GalaSoft.MvvmLight;
using StorAmp.Core.Model.Clipboard;
using GalaSoft.MvvmLight.Messaging;
using StorAmp.Core.Model.Messages;
using NetBox.Extensions;
using Serilog;

namespace StorAmp.Core.ViewModel
{
   public partial class BlobStoragePanelViewModel : ViewModelBase, IAccountPaneViewModel
   {
      public const string ParentFolderName = "...";

      public event PropertyChangedEventHandler PropertyChanged;

      public event Action<int> BlobsReloaded;

      public ObservableCollection<Blob> Blobs { get; } = new ObservableCollection<Blob>();

      public ObservableCollection<Blob> FilteredBlobs { get; } = new ObservableCollection<Blob>();

      public ObservableCollection<Blob> SelectedBlobs { get; } = new ObservableCollection<Blob>();

      public IBlobStorage Storage { get; private set; }

      public ViewEditControlViewModel ViewEdit { get; } = new ViewEditControlViewModel();

      public ClipboardViewModel Clipboard { get; } = ClipboardViewModel.Instance;

      public FolderBrowserViewModel FolderBrowser { get; } = new FolderBrowserViewModel();

      public BlobStoragePanelViewModel(ConnectedAccount storageAccount)
      {
         Account = storageAccount;
         Storage = storageAccount.CreateBlobStorage();
         FolderBrowser.Storage = Storage;

         ViewEdit.OnDone += (done) =>
         {
            IsViewing = false;
         };

         SelectedBlobs.CollectionChanged += SelectedBlobs_CollectionChanged;
         Clipboard.PropertyChanged += (_, args) =>
         {
            if(args.PropertyName == nameof(ClipboardViewModel.HasData))
            {
               PasteFromClipboardCommand?.RaiseCanExecuteChanged();
            }
         };

         FolderBrowser.OnDoubleTapFolder += FolderBrowser_OnDoubleTapFolder;

         GlobalSettings.Default.PropertyChanged += ConfigPropertyChanged;

         Messenger.Default.Register<FolderUpdatedMessage>(this, OnFolderUpdated);

         CreateCommands();
      }

      private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         string pn = e.PropertyName;

         if(pn == nameof(ICloudExplorerSettings.AlternateRowColours) ||
            pn == nameof(ICloudExplorerSettings.HumanisedBlobChangeDates) ||
            pn == nameof(ICloudExplorerSettings.FoldersFirst) ||
            pn == nameof(ICloudExplorerSettings.SingleClickNavigation))
         {
            RefreshBlobsAsync().Forget();
         }
      }

      private void FolderBrowser_OnDoubleTapFolder(Blob folder)
      {
         FolderPath = folder.FullPath;
      }

      private void OnFolderUpdated(FolderUpdatedMessage message)
      {
         if(message.BlobStorage == Storage && StoragePath.ComparePath(message.FolderPath, FolderPath))
         {
            ServiceLocator.GetInstance<IUIDispatcher>().Invoke(() => Process(message));
         }
      }

      private void Process(FolderUpdatedMessage message)
      {
         if(message.ReplacedBlobs.Count > 0)
         {
            foreach(KeyValuePair<Blob, Blob> otn in message.ReplacedBlobs)
            {
               int idx = FilteredBlobs.IndexOf(otn.Key);
               if(idx != -1)
               {
                  FilteredBlobs.RemoveAt(idx);
                  FilteredBlobs.Insert(idx, otn.Value);
               }
            }
         }
         else
         {
            RefreshBlobsAsync().Forget();
         }
      }

      private void SelectedBlobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         SelectedBlob = SelectedBlobs.Count == 1 ? SelectedBlobs[0] : null;

         DeleteBlobsCommand?.RaiseCanExecuteChanged();
         ViewBlobCommand?.RaiseCanExecuteChanged();
         RenameCommand?.RaiseCanExecuteChanged();
         ExternalOpenCommand?.RaiseCanExecuteChanged();
         CopySelectedBlobsCommand?.RaiseCanExecuteChanged();
         ShowBlobPropertiesCommand?.RaiseCanExecuteChanged();
         DownloadBlobsCommand?.RaiseCanExecuteChanged();
      }

      private IProgressReportingService ProgressReportingService => ServiceLocator.GetInstance<IProgressReportingService>();

      private IDialogService DialogService => ServiceLocator.GetInstance<IDialogService>();

      private ITaskManagerService TaskService => ServiceLocator.GetInstance<ITaskManagerService>();

      private bool HasSelection => SelectedBlobs.Count > 0;

      private bool HasOneFileSelected => SelectedBlobs.Count == 1 && SelectedBlobs.First().IsFile;

      private bool HasOneFileOrFolderSelected => SelectedBlobs.Count == 1 && SelectedBlobs.First().Name != ParentFolderName;

      private async Task CreateFolderAsync()
      {
         string name = await DialogService.AskStringInputAsync("New Folder", "Folder Name");

         if(name != null)
         {
            EventLog.LogEvent("createFolder", "{name}", name);
            await Storage.CreateFolderAsync(StoragePath.Combine(FolderPath, name));
            await RefreshBlobsAsync();
         }
      }

      private async Task RenameAsync()
      {
         string newName = await DialogService.AskStringInputAsync("Rename", "New Name", SelectedBlob.Name);

         if(newName == null)
            return;

         string newPath = StoragePath.Combine(StoragePath.GetParent(SelectedBlob.FullPath), newName);

         EventLog.LogEvent("rename", SelectedBlob.FullPath, newPath);

         await TaskService.ScheduleAsync(new RenameFilesTask(Storage, SelectedBlob, newPath));
      }

      private async Task CreateTextFileAsync()
      {
         string name = await DialogService.AskStringInputAsync("New File", "File Name");

         if(name != null)
         {
            EventLog.LogEvent("createTextFile", "{name}", name);
            await Storage.WriteTextAsync(StoragePath.Combine(FolderPath, name), $"type your text here");
            await RefreshBlobsAsync();
         }
      }

      public ConnectedAccount Account { get; private set; }

      public void RestoreSettings(string settings)
      {
         FolderPath = string.IsNullOrEmpty(settings) ? StoragePath.RootFolderPath : settings;
      }

      public string GetSettings()
      {
         return FolderPath;
      }

      public async Task RefreshBlobsAsync()
      {
         if(Storage == null)
            return;

         SingleClickNavigation = GlobalSettings.Default.SingleClickNavigation;

         IsLoading = true;

         try
         {
            List<Blob> items = (await Storage.ListAsync(recurse: false, folderPath: FolderPath, includeAttributes: true)).ToList();
            var finalItems = new List<Blob>(items.Count);
            if(GlobalSettings.Default.FoldersFirst)
            {
               finalItems.AddRange(items.Where(i => i.IsFolder).OrderBy(f => f.Name));
               finalItems.AddRange(items.Where(i => i.IsFile).OrderBy(f => f.Name));
            }
            else
            {
               finalItems = items;
            }

            if(!StoragePath.IsRootPath(FolderPath))
            {
               //can't use ".." as it has a special meaning
               var parentBlob = new Blob(ParentFolderName, BlobItemKind.Folder);
               if(finalItems.Count == 0)
                  finalItems.Add(parentBlob);
               else
                  finalItems.Insert(0, parentBlob);
            }

            Blobs.AddAll(finalItems, true);

            RefreshStatusBar();

            FilterBlobs();

            HasError = false;

            BlobsReloaded?.Invoke(Blobs.Count);
         }
         catch(Exception ex)
         {
            HasError = true;
            ErrorMessage = ex.Message;
            ErrorDetails = ex.StackTrace;
         }
         finally
         {
            IsLoading = false;
         }
      }

      private void FilterBlobs()
      {
         FilteredBlobs.Clear();

         if(string.IsNullOrEmpty(FilterText))
         {
            FilteredBlobs.AddAll(Blobs);
         }
         else
         {
            FilteredBlobs.AddAll(Blobs.Where(b => b.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) != -1).ToList());
         }
      }

      private void RefreshStatusBar()
      {
         TotalSize = Blobs
            .Where(b => b.Kind == BlobItemKind.File)
            .Where(b => b.Size != null)
            .Sum(b => b.Size.Value);
      }

      public async Task ItemActionAsync(Blob blob)
      {
         if(blob.Kind == BlobItemKind.Folder)
         {
            //GoLevelUpCommand.Execute(null);
            FolderPath = StoragePath.Normalize(
               StoragePath.Combine(FolderPath, blob.Name == ParentFolderName ? StoragePath.LevelUpFolderName : blob.Name), true);
         }
         else
         {
            ViewBlobCommand.Execute(null);
         }
      }

      public async Task ExternalOpenAsync(Blob blob)
      {
         ITaskManagerService tm = ServiceLocator.GetInstance<ITaskManagerService>();
         await tm.ScheduleAsync(new DownloadAndViewLocallyTask(Storage, blob));
      }

      public async Task DeleteBlobsAsync()
      {
         if(SelectedBlobs.Count == 0) return;

         string message = SelectedBlobs.Count == 1
            ? $"'{SelectedBlobs.First().Name}'"
            : $"{"item".ToQuantity(SelectedBlobs.Count)}";

         if(await DialogService.AskYesNoAsync(
            Strings.BackgroundTask_Delete_DialogTitle, "Would you like to delete " + message + "?"))
         {
            await TaskService.ScheduleAsync(new DeleteFilesTask(Account, Storage, SelectedBlobs));
         }
      }

      public void GoLevelUp()
      {
         FolderPath = StoragePath.GetParent(FolderPath);
      }

      #region [ Drag & Drop ]

      public bool AcceptsDrop(DragData data)
      {
         //deny drop onto myself
         return data.Account.Id != Account.Id;
      }


      public async void DropDataAsync(DragData data)
      {
         var storage = (IBlobStorage)data.Properties["storage"];
         var blobs = (List<Blob>)data.Properties["blobs"];

         if(await DialogService?.AskYesNoAsync("Copy Files",
            $"Copy {"item".ToQuantity(blobs.Count)} to {Account.DisplayName}{FolderPath}?"))
         {
            var copyTask = new CopyFilesTask(storage, blobs, Storage, FolderPath);
            await TaskService.ScheduleAsync(copyTask);
         }
      }

      #endregion

      #region [ Copy Operations ]

      public async Task UploadLocalFilesAsync(IReadOnlyCollection<string> filePaths)
      {
         var task = new UploadFilesTask(Storage, FolderPath, filePaths);
         await TaskService.ScheduleAsync(task);
      }

      private async Task PaseFromClipboardAsync()
      {
         BlobsClipboardData data = ClipboardViewModel.Instance.Data;

         if(await DialogService?.AskYesNoAsync("Copy Files",
            $"Copy {"item".ToQuantity(data.Blobs.Count)} to {Account.DisplayName}{FolderPath}?"))
         {
            var copyTask = new CopyFilesTask(data.Storage, data.Blobs, Storage, FolderPath);
            await TaskService.ScheduleAsync(copyTask);
         }
      }

      #endregion
      public override string ToString() => Account.DisplayName;
   }
}