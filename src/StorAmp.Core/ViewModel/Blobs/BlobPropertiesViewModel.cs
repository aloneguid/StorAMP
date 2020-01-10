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
   public class BlobPropertiesViewModel : ViewModelBase
   {
      public BindingList<EditableKeyValue> EditableMetadata { get; } = new BindingList<EditableKeyValue>();

      private IDialogService DialogService => ServiceLocator.GetInstance<IDialogService>();

      public BlobPropertiesViewModel()
      {
         EditableMetadata.ListChanged += EditableMetadata_ListChanged;
      }

      private void EditableMetadata_ListChanged(object sender, ListChangedEventArgs e)
      {
         IsDirty = true;
      }

      private async Task LoadFullBlobAsync()
      {
         if(SelectedBlob == null || Storage == null)
            return;

         IsLoading = true;
         EditableMetadata.Clear();
         try
         {
            FullBlob = await Storage.GetBlobAsync(SelectedBlob);
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
         if(FullBlob == null || Storage == null)
            return;

         Blob b = FullBlob;
         b.Metadata.Clear();
         b.Metadata.AddRange(EditableKeyValue.ToDictionary(EditableMetadata));
         IsLoading = true;
         try
         {
            await Storage.SetBlobAsync(b);
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
                }));
         }
      }

      #endregion


      #region [ Properties ]

      private IBlobStorage _storage;
      public IBlobStorage Storage
      {
         get => _storage;
         set
         {
            _storage = value;
         }
      }


      private ConnectedAccount _storageAccount;
      public ConnectedAccount StorageAccount
      {
         get => _storageAccount;
         set { Set(() => StorageAccount, ref _storageAccount, value);}
      }


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
         set { Set(() => IsDirty, ref _isDirty, value); }
      }

      private Blob _selectedBlob;
      public Blob SelectedBlob
      {
         get => _selectedBlob;
         set
         {
            Set(() => SelectedBlob, ref _selectedBlob, value);
            FullBlob = null;
            LoadFullBlobAsync().Forget();
         }
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
