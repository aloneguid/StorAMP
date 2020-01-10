using System.Windows.Controls;
using StorAmp.Core.ViewModel.Account;

namespace StorAmp.Wpf.Wpf.Dialogs
{
   /// <summary>
   /// Interaction logic for AddAwsAccountDialog.xaml
   /// </summary>
   public partial class AddAwsAccountDialog : UserControl
   {
      public AddAwsAccountDialog()
      {
         InitializeComponent();
      }

      public AddAwsAccountViewModel ViewModel => (AddAwsAccountViewModel)DataContext;
   }
}
