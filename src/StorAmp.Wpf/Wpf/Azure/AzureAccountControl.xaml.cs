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
using Serilog;
using StorAmp.Core.ViewModel.Azure;

namespace StorAmp.Wpf.Wpf.Azure
{
   /// <summary>
   /// Interaction logic for AzureAccountControl.xaml
   /// </summary>
   public partial class AzureAccountControl : UserControl
   {
      public AzureAccountControl()
      {
         InitializeComponent();
      }

      private AzureAccountControlViewModel ViewModel => (AzureAccountControlViewModel)DataContext;

      private void CopyCode_Click(object sender, RoutedEventArgs e)
      {
         ViewModel.UserCode.CopyToClipboard();

         EventLog.LogEvent("azureLogin", "action: {action}", "copyUserCode");
      }

      private void CopyCodeAndOpenBrowser_Click(object sender, RoutedEventArgs e)
      {
         ViewModel.UserCode.CopyToClipboard();
         ViewModel.VerificationUrl.ShellOpen();

         EventLog.LogEvent("azureLogin", "action: {action}", "copyUserCodeAndOpenBrowser");
      }

      private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
      {
         ViewModel.VerificationUrl.ShellOpen();

         EventLog.LogEvent("azureLogin", "action: {action}", "openBrowser");
      }
   }
}
