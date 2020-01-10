using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetBox.Extensions;
using StorAmp.Core.Services;
using StorAmp.Core.ViewModel;
using StorAmp.Core.ViewModel.Blobs;
using StorAmp.Wpf.Wpf.Blobs;

namespace CloudExplorer.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for ActionToolbar.xaml
   /// </summary>
   public partial class ActionToolbar : UserControl
   {
      public ActionToolbar()
      {
         InitializeComponent();
      }

      private BlobStoragePanelViewModel ViewModel => (BlobStoragePanelViewModel)DataContext;

      private void ShowMetadata_Click(object sender, RoutedEventArgs e)
      {
         var vm = new BlobMetadataViewModel(ViewModel.Storage, ViewModel.SelectedBlob);
         var metaControl = new BlobMetadataContent(vm);

         ServiceLocator.GetInstance<IDialogService>().ShowDialogAsync("Metadata", metaControl, "Update", vm.SaveCommand).Forget();
      }
   }
}
