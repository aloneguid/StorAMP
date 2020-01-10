using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Storage.Net.Blobs;
using StorAmp.Core.Model;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Blobs
{
   public class BlobMetadataViewModel : ViewModelBase
   {
      private readonly IBlobStorage _blobStorage;
      private readonly Blob _blob;

      public BindingList<EditableKeyValue> EditableMetadata { get; } = new BindingList<EditableKeyValue>();

      private IDialogService DialogService => ServiceLocator.GetInstance<IDialogService>();

      public BlobMetadataViewModel(IBlobStorage blobStorage, Blob blob)
      {
         EditableMetadata.ListChanged += EditableMetadata_ListChanged;
         _blobStorage = blobStorage;
         _blob = blob;

         LoadFullBlobAsync().Forget();
      }

      private void EditableMetadata_ListChanged(object sender, ListChangedEventArgs e)
      {
         IsDirty = true;
      }

      private async Task LoadFullBlobAsync()
      {
         if(_blob == null || _blobStorage == null)
            return;

         IsLoading = true;
         EditableMetadata.Clear();
         try
         {
            FullBlob = await _blobStorage.GetBlobAsync(_blob);
            if(FullBlob != null)
            {
               EditableMetadata.AddRange(FullBlob.Metadata.Select(p => new EditableKeyValue { Key = p.Key, Value = p.Value }));
            }
         }
         catch(Exception ex)
         {
            await DialogService.ShowMessageAsync("Failed to load", ex.Message);
         }
         finally
         {
            IsDirty = false;
            IsLoading = false;
         }
      }

      private async Task SaveAsync()
      {
         if(FullBlob == null || _blobStorage == null)
            return;

         Blob b = FullBlob;
         b.Metadata.Clear();
         b.Metadata.AddRange(EditableKeyValue.ToDictionary(EditableMetadata));
         IsLoading = true;
         try
         {
            await _blobStorage.SetBlobAsync(b);
         }
         catch(Exception ex)
         {
            await DialogService.ShowMessageAsync("Failed to update", ex.Message);
         }
         finally
         {
            IsLoading = false;
            await LoadFullBlobAsync();
         }

      }

      #region [ Commands ]

      private RelayCommand _saveCommand;

      public RelayCommand SaveCommand
      {
         get
         {
            return _saveCommand
                ?? (_saveCommand = new RelayCommand(
                () =>
                {
                   SaveAsync().Forget();
                }, () => IsDirty));
         }
      }

      #endregion


      #region [ Properties ]

      private bool _isLoading;
      public bool IsLoading
      {
         get => _isLoading;
         set { Set(() => IsLoading, ref _isLoading, value); }
      }


      private bool _isDirty;
      public bool IsDirty
      {
         get => _isDirty;
         set { Set(() => IsDirty, ref _isDirty, value); SaveCommand?.RaiseCanExecuteChanged(); }
      }

      private Blob _fullBlob;
      public Blob FullBlob
      {
         get => _fullBlob;
         set { Set(() => FullBlob, ref _fullBlob, value); }
      }

      #endregion
   }
}
