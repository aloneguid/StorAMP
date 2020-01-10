using System.Windows;
using System.Windows.Controls;
using StorAmp.Core.ViewModel;
using StorAmp.Wpf.Wpf.Viewers;
using System.Linq;
using Storage.Net.Blobs;
using System;
using Serilog;
using System.Collections.Generic;
using System.IO;
using StorAmp.Core;

namespace CloudExplorer.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for ViewEditControl.xaml
   /// </summary>
   public partial class ViewEditControl : UserControl
   {
      private IFileViewer _viewer;

      public ViewEditControl()
      {
         InitializeComponent();
      }

      private ViewEditControlViewModel ViewModel => (ViewEditControlViewModel)DataContext;

      private static bool IsExtension(string commaSeparatedList, string extensionToCheck)
      {
         var hash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
         foreach(string ext in commaSeparatedList.Split(','))
         {
            string extClean = ext.Trim();
            if(extClean != string.Empty)
            {
               hash.Add(extClean);
            }
         }

         return hash.Contains(extensionToCheck);
      }

      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         if(ViewModel == null)
            return;

         ViewModel.OnView += ViewModel_OnView;
         ViewModel.OnClearView += ViewModel_OnClearView;
         ViewModel.OnDone += ViewModel_OnDone;
         ViewModel.OnSave += _ =>
         {
            if(_viewer is IFileEditor editor)
            {
               editor.SaveContentToFile(ViewModel.TempFilePath);
            }
         };
      }

      private void ViewModel_OnDone(bool obj)
      {
         _viewer?.Stop();
      }

      private void ViewModel_OnClearView(bool obj)
      {
         ViewerContent.Children.Clear();
      }

      private void ViewModel_OnView(Blob blob, string filePath, string extension)
      {
         RenderFile(blob, filePath, extension);

         CloseButton.Focus();
      }

      private void RenderFile(Blob blob, string filePath, string extension)
      {
         ViewModel.ErrorText = null;

         if(blob == null)
            return;

         ViewerContent.Children.Clear();

         UserControl fileViewerControl = CreateViewer(extension ?? Path.GetExtension(blob));

         _viewer = (IFileViewer)fileViewerControl;
         if(_viewer is IFileEditor editor)
         {
            editor.FileChanged += _ => ViewModel.HasChanged = true;
         }

         try
         {
            _viewer.SetContentFromFile(filePath, extension);
            ViewerContent.Children.Add(fileViewerControl);
         }
         catch(Exception ex)
         {
            ViewModel.ErrorText = ex.Message;
            Log.Error(ex, "failed to render");
         }
      }

      private UserControl CreateViewer(string extension)
      {
         if(extension != null)
         {
            if(IsExtension(GlobalSettings.Default.ImageExtensions, extension))
            {
               return new PictureViewer();
            }
            else if(IsExtension(GlobalSettings.Default.VideoExtensions, extension))
            {
               return new VideoViewer();
            }
            else if(extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            {
               return new CsvViewer();
            }
         }

         return new TextViewer();
      }
   }
}
