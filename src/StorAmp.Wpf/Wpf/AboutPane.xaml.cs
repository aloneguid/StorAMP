using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for AboutPane.xaml
   /// </summary>
   public partial class AboutPane : UserControl
   {
      public AboutPane()
      {
         InitializeComponent();
      }

      private void Nav(object sender, RequestNavigateEventArgs e)
      {
         e.Uri.AbsoluteUri.ShellOpen();
      }
   }
}
