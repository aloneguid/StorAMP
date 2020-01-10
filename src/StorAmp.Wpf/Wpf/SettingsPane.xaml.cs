using System.Windows.Controls;
using System.Windows.Input;

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for SettingsPane.xaml
   /// </summary>
   public partial class SettingsPane : UserControl
   {
      public SettingsPane()
      {
         InitializeComponent();
      }

      private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         ((TextBox)sender).Text.ShellOpen();
      }
   }
}
