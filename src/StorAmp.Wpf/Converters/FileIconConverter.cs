using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CloudExplorer.Wpf.Interop;
using Storage.Net.Blobs;
using StorAmp.Core.ViewModel.Blobs;

namespace StorAmp.Wpf.Converters
{
   public class FileIconConverter : IValueConverter
   {
      private static readonly ImageSource _folderImageSource;

      static FileIconConverter()
      {
         _folderImageSource = "folder".GetImageSource();
      }

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(value == null)
            return null;

         if(!(value is Blob blob))
            return null;

         if(FolderResource.IsLoadingBlob(blob))
            return null;

         if(targetType == typeof(ImageSource))
         {
            if(blob.Kind == BlobItemKind.Folder)
            {
               if(blob.Properties.ContainsKey("IsLogsContainer"))
                  return "azure/metrics".GetImageSource();

               if(blob.Properties.ContainsKey("IsSecret"))
                  return "account/azure.keyvault.secret".GetImageSource();

               if(blob.Properties.ContainsKey("IsContainer"))
                  return "account/azure.blob.container".GetImageSource();

               if(blob.Properties.ContainsKey("IsFilesystem"))
                  return "account/azure.datalake.gen2".GetImageSource();

               if(blob.Properties.ContainsKey("IsShare"))
                  return "account/azure.file".GetImageSource();


               return _folderImageSource;
            }

            return WindowsNative.HasAssociatedProgram(blob.Name) ? IconManager.FindIconForFilename(blob.Name, false) : null;
         }
         else if(targetType == typeof(Visibility))
         {
            if(blob.IsFile && WindowsNative.HasAssociatedProgram(blob.Name))
               return Visibility.Visible;

            return Visibility.Collapsed;
         }

         return null;

      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
