using System.Windows.Controls;
using Storage.Net.Blobs;
using StorAmp.Core.ViewModel.Blobs;

namespace StorAmp.Wpf.Wpf.Blobs
{
   /// <summary>
   /// Interaction logic for BlobProperties.xaml
   /// </summary>
   public partial class BlobMetadataContent : UserControl
   {
      public BlobMetadataContent(BlobMetadataViewModel vm)
      {
         InitializeComponent();

         this.DataContext = vm;
      }
   }
}
