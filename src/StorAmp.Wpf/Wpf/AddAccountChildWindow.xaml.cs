using System.Windows;
using System.Windows.Input;
using MahApps.Metro.SimpleChildWindow;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Account;
using StorAmp.Core.ViewModel;

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for AddAccountChildWindow.xaml
   /// </summary>
   public partial class AddAccountChildWindow : ChildWindow
   {
      private readonly ConnectedFolder _connectedFolder;

      public AddAccountChildWindow(ConnectedFolder connectedFolder, ConnectedAccount connectedAccount)
      {
         InitializeComponent();

         this.Title = connectedAccount == null
            ? "Add New Account"
            : "Edit Account";

         this.DataContext = new AccountsViewModel(connectedFolder, connectedAccount);

         ViewModel.Committed += _ => Close();
         ViewModel.AccountTypeSelected = connectedAccount != null;
         _connectedFolder = connectedFolder;
      }

      private AccountsViewModel ViewModel => (AccountsViewModel)DataContext;

      private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         var uie = (FrameworkElement)sender;
         AccountDefinition ad = (AccountDefinition)uie.DataContext;
         ViewModel.AddCommand.Execute(ad);
      }
   }
}
