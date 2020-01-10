using System.Collections.ObjectModel;
using System.Linq;
using StorAmp.Core.Model;
using Storage.Net.Blobs;

namespace StorAmp.Core.ViewModel
{
   public partial class ViewEditControlViewModel
   {
      public ObservableCollection<FileViewerDetails> Viewers { get; } = new ObservableCollection<FileViewerDetails>();


      private FileViewerDetails _selectedViewer;
      public FileViewerDetails SelectedViewer
      {
         get => _selectedViewer;
         set
         {
            Set(() => SelectedViewer, ref _selectedViewer, value);

            OnClearView?.Invoke(true);
            OnView?.Invoke(SelectedBlob, TempFilePath, value?.Extensions.First());
         }
      }

      private string _tempFilePath;
      public string TempFilePath
      {
         get => _tempFilePath;
         set { Set(() => TempFilePath, ref _tempFilePath, value); }
      }


      private Blob _selectedBlob;
      public Blob SelectedBlob
      {
         get => _selectedBlob;
         set { Set(() => SelectedBlob, ref _selectedBlob, value); }
      }


      private bool _isLoading;
      public bool IsLoading
      {
         get => _isLoading;
         set { Set(() => IsLoading, ref _isLoading, value); }
      }

      private string _errorText;
      public string ErrorText
      {
         get => _errorText;
         set { Set(() => ErrorText, ref _errorText, value); }
      }


      private bool _hasChanged;
      public bool HasChanged
      {
         get => _hasChanged;
         set { Set(() => HasChanged, ref _hasChanged, value); SaveCommand?.RaiseCanExecuteChanged(); }
      }
   }
}
