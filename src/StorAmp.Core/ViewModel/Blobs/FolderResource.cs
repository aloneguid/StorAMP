using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NetBox.Extensions;
using Storage.Net.Blobs;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Blobs
{
   public class FolderResource : HierarchicalResource
   {
      private const string LoadingBlobPropertyIdentifier = "IsDynamicallyLoading";
      private readonly IBlobStorage _storage;

      public FolderResource(IBlobStorage storage, Blob blob) : base(blob.Name, "folder")
      {
         _storage = storage;
         Blob = blob;
         IsExpanded = false;
         if(!IsLoadingBlob(blob))
         {
            Children.Add(new FolderResource(_storage, CreateLoadingBlob()));
         }
         PropertyChanged += FolderResource_PropertyChanged;

         AddCommandGroup(
            new HierarchicalResourceCommand("refresh", Symbol.Refresh, () => LoadChildrenAsync(true)),
            new HierarchicalResourceCommand("delete", Symbol.Delete, DeleteFolderAsync));
      }

      private void FolderResource_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if(e.PropertyName == nameof(IsExpanded))
         {
            if(IsExpanded)
            {
               LoadChildrenAsync(false).Forget();
            }
         }
      }

      private async Task DeleteFolderAsync()
      {

      }

      private bool HasOnlyLoadingBlob
      {
         get
         {
            if(Children.Count != 1)
               return false;

            Blob first = ((FolderResource)(Children.First())).Blob;

            return IsLoadingBlob(first);
         }
      }

      public static bool IsLoadingBlob(Blob blob)
      {
         return blob.TryGetProperty(LoadingBlobPropertyIdentifier, out bool v);
      }

      private async Task LoadChildrenAsync(bool force)
      {
         if(!force && !HasOnlyLoadingBlob)
            return;

         IsLoading = true;
         try
         {
            IReadOnlyCollection<Blob> subfolders = await _storage.ListAsync(folderPath: Blob.FullPath, browseFilter: b => b.IsFolder);

            Children.Clear();
            foreach(Blob folder in subfolders)
            {
               Children.Add(new FolderResource(_storage, folder));
            }

            Error = null;
         }
         catch(Exception ex)
         {
            Error = ex;
         }
         finally
         {
            IsLoading = false;
         }
      }

      private static Blob CreateLoadingBlob()
      {
         var blob = new Blob(Strings.LoadingBlobName, BlobItemKind.Folder);
         blob.Properties.Add(LoadingBlobPropertyIdentifier, true);
         return blob;
      }

      public Blob Blob { get; private set; }
   }
}
