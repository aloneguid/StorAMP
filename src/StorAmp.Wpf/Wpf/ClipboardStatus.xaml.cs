using System.Windows.Controls;
using StorAmp.Core.ViewModel;

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for ClipboardStatus.xaml
   /// </summary>
   public partial class ClipboardStatus : UserControl
   {
      public ClipboardStatus()
      {
         InitializeComponent();

         this.DataContext = ViewModel;
      }

      public ClipboardViewModel ViewModel { get; } = ClipboardViewModel.Instance;
   }
}
