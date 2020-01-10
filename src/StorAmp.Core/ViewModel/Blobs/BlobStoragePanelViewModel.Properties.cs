using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Humanizer;
using NetBox.Extensions;
using Storage.Net;
using Storage.Net.Blobs;

namespace StorAmp.Core.ViewModel
{
   public partial class BlobStoragePanelViewModel
   {

      private Blob _selectedBlob;
      public Blob SelectedBlob
      {
         get => _selectedBlob;
         set
         {
            _selectedBlob = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedBlob)));
         }
      }

      private long _totalSize;

      public long TotalSize
      {
         get => _totalSize;
         set
         {
            _totalSize = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalSize)));
         }
      }

      private bool _isLoading;
      public bool IsLoading
      {
         get => _isLoading;
         set
         {
            _isLoading = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
         }
      }

      private bool _isViewing;
      public bool IsViewing
      {
         get => _isViewing;
         set
         {
            _isViewing = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsViewing)));
         }
      }


      private bool _isAnalysing;
      public bool IsAnalysing
      {
         get => _isAnalysing;
         set { Set(() => IsAnalysing, ref _isAnalysing, value); }
      }

      private string _folderPath;
      public string FolderPath
      {
         get => _folderPath;
         set
         {
            _folderPath = value ?? StoragePath.RootFolderPath;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FolderPath)));
            RefreshBlobsAsync().Forget();
            GoLevelUpCommand.RaiseCanExecuteChanged();
         }
      }

      private bool _hasError;
      public bool HasError
      {
         get => _hasError;
         set
         {
            _hasError = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasError)));
         }
      }


      private string _errorMessage;
      public string ErrorMessage
      {
         get => _errorMessage;
         set
         {
            _errorMessage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            ErrorMessageShort = value == null
               ? null
               : value.Truncate(50).Replace("\r", " ").Replace("\n", " ");
         }
      }


      private string _ErrorMessageShort;
      public string ErrorMessageShort
      {
         get => _ErrorMessageShort;
         set { Set(() => ErrorMessageShort, ref _ErrorMessageShort, value); }
      }


      private string _errorDetails;
      public string ErrorDetails
      {
         get => _errorDetails;
         set
         {
            _errorDetails = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorDetails)));
         }
      }


      private string _filterText;
      public string FilterText
      {
         get => _filterText;
         set { Set(() => FilterText, ref _filterText, value); FilterBlobs(); }
      }


      private bool _SingleClickNavigation;
      public bool SingleClickNavigation
      {
         get => _SingleClickNavigation;
         set { Set(() => SingleClickNavigation, ref _SingleClickNavigation, value); }
      }

   }
}
