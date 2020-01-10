using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Azure.KeyVault.Models;
using NetBox.Extensions;
using Storage.Net.Blobs;

namespace StorAmp.Core.ViewModel.Blobs
{


   public class FolderBrowserViewModel : ViewModelBase
   {
      public event Action<Blob> OnDoubleTapFolder;

      public ObservableCollection<HierarchicalResource> RootFolders { get; set; } = new ObservableCollection<HierarchicalResource>();

      public FolderBrowserViewModel()
      {

      }


      private async Task RefreshRootAsync()
      {
         if(Storage == null)
            return;

         IsLoading = true;
         try
         {
            IReadOnlyCollection<Blob> roots = (await Storage.ListAsync(recurse: false, browseFilter: b => b.IsFolder));

            RootFolders.Clear();

            foreach(Blob folder in roots)
            {
               RootFolders.Add(new FolderResource(Storage, folder));
            }
         }
         finally
         {
            IsLoading = false;
         }
      }

      public void DoubleTapFolder(Blob blob)
      {
         OnDoubleTapFolder?.Invoke(blob);
      }

      private IBlobStorage _Storage;
      public IBlobStorage Storage
      {
         get => _Storage;
         set { Set(() => Storage, ref _Storage, value); RefreshRootAsync().Forget(); }
      }


      private bool _IsLoading;
      public bool IsLoading
      {
         get => _IsLoading;
         set { Set(() => IsLoading, ref _IsLoading, value); }
      }

   }
}
