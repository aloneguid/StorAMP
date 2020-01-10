using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StorAmp.Core.ViewModel;
using Storage.Net.Blobs;

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for BlobStoragePanel.xaml
   /// </summary>
   public partial class BlobStoragePanel : UserControl
   {
      public BlobStoragePanel()
      {
         InitializeComponent();
      }

      private BlobStoragePanelViewModel ViewModel => (BlobStoragePanelViewModel)DataContext;
   }
}