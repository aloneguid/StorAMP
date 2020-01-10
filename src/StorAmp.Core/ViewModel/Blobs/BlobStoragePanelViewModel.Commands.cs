using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Serilog;
using Storage.Net;
using Storage.Net.Blobs;
using StorAmp.Core.Model.Clipboard;
using StorAmp.Core.Services;
using StorAmp.Core.Tasks;

namespace StorAmp.Core.ViewModel
{
   public partial class BlobStoragePanelViewModel
   {
      private void CreateCommands()
      {
         GoLevelUpCommand = new RelayCommand(() =>
         {
            GoLevelUp();
         },
         () => !StoragePath.IsRootPath(FolderPath));

         ExternalOpenCommand = new RelayCommand(() =>
         {
            ExternalOpenAsync(SelectedBlob).Forget();
         },
         () => HasOneFileSelected);
      }

      private RelayCommand _viewBlobCommand;
      public RelayCommand ViewBlobCommand
      {
         get
         {
            return _viewBlobCommand
                ?? (_viewBlobCommand = new RelayCommand(
                () =>
                {
                   IsViewing = true;
                   ViewEdit.PreviewAsync(Storage, SelectedBlobs.First()).Forget();
                },
                () => HasOneFileSelected));
         }
      }

      private RelayCommand _RenameCommand;
      public RelayCommand RenameCommand
      {
         get
         {
            return _RenameCommand
                ?? (_RenameCommand = new RelayCommand(
                () =>
                {
                   RenameAsync().Forget();
                },
                () => HasOneFileOrFolderSelected));
         }
      }

      private RelayCommand _createTextFileCommand;
      public RelayCommand CreateTextFileCommand
      {
         get
         {
            return _createTextFileCommand
                ?? (_createTextFileCommand = new RelayCommand(
                () =>
                {
                   CreateTextFileAsync().Forget();
                }));
         }
      }

      public RelayCommand ExternalOpenCommand { get; private set; }

      public ICommand CreateFolderCommand => new RelayCommand(() =>
      {
         CreateFolderAsync().Forget();
      });

      public ICommand RefreshCommand => new RelayCommand(() =>
      {
         RefreshBlobsAsync().Forget();

         EventLog.LogEvent("refreshBlobs", "{prefix}", Account.Prefix);
      });

      public RelayCommand GoLevelUpCommand { get; private set; }

      private RelayCommand _deleteBlobsCommand;
      public RelayCommand DeleteBlobsCommand
      {
         get
         {
            return _deleteBlobsCommand
                ?? (_deleteBlobsCommand = new RelayCommand(
                () =>
                {
                   DeleteBlobsAsync().Forget();
                },
                () => HasSelection));
         }
      }

      private RelayCommand _copySelectedBlobsCommand;

      public RelayCommand CopySelectedBlobsCommand
      {
         get
         {
            return _copySelectedBlobsCommand
                ?? (_copySelectedBlobsCommand = new RelayCommand(
                () =>
                {
                   var data = new BlobsClipboardData(Account, Storage, SelectedBlobs);
                   ClipboardViewModel.Instance.Push(data);
                },
                () => HasSelection));
         }
      }

      private RelayCommand _pasteFromClipboardCommand;

      /// <summary>
      /// Gets the PasteFromClipboardCommand.
      /// </summary>
      public RelayCommand PasteFromClipboardCommand
      {
         get
         {
            return _pasteFromClipboardCommand
                ?? (_pasteFromClipboardCommand = new RelayCommand(
                () =>
                {
                   PaseFromClipboardAsync().Forget();
                },
                () => Clipboard.HasData));
         }
      }

      public ICommand ItemActionCommand => new RelayCommand(() =>
      {
         if(!HasSelection)
            return;

         ItemActionAsync(SelectedBlobs.First()).Forget();
      });

      private RelayCommand<Blob> _ParametrisedItemActionCommand;
      public RelayCommand<Blob> ParametrisedItemActionCommand
      {
         get
         {
            return _ParametrisedItemActionCommand
                ?? (_ParametrisedItemActionCommand = new RelayCommand<Blob>(
                p =>
                {
                   if(p == null)
                      return;

                   ItemActionAsync(p).Forget();
                }));
         }
      }

      private RelayCommand _uploadFromDiskCommand;

      /// <summary>
      /// Gets the UploadFromDiskCommand.
      /// </summary>
      public RelayCommand UploadFromDiskCommand
      {
         get
         {
            return _uploadFromDiskCommand
                ?? (_uploadFromDiskCommand = new RelayCommand(
                () =>
                {
                   IReadOnlyCollection<string> files = DialogService.AskLocalFile("Select files to upload");

                   if(files == null)
                      return;

                   UploadLocalFilesAsync(files).Forget();
                },
                () => true));
         }
      }

      private RelayCommand _DownloadBlobsCommand;
      public RelayCommand DownloadBlobsCommand
      {
         get
         {
            return _DownloadBlobsCommand
                ?? (_DownloadBlobsCommand = new RelayCommand(
                () =>
                {
                   string destPath = DialogService.AskLocalFolder("Select destination folder");

                   if(destPath == null)
                      return;

                   var task = new CopyFilesTask(
                     Storage, SelectedBlobs.ToList(),
                     StorageFactory.Blobs.DirectoryFiles(destPath),
                     StoragePath.RootFolderPath);

                   TaskService.ScheduleAsync(task).Forget();
                },
                () => SelectedBlobs.Count > 0));
         }
      }

      private RelayCommand _ShowBlobPropertiesCommand;
      public RelayCommand ShowBlobPropertiesCommand
      {
         get
         {
            return _ShowBlobPropertiesCommand
                ?? (_ShowBlobPropertiesCommand = new RelayCommand(
                () =>
                {
                   ServiceLocator.GetInstance<IDialogService>().ShowPropertiesAsync(
                      "Properties", SelectedBlob.Properties.ToDictionary(p => p.Key, p => (object)p.Value)).Forget();
                },
                () => SelectedBlob != null));
         }
      }
   }
}
