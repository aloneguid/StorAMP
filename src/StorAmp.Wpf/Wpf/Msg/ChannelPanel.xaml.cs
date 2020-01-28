using System.Windows;
using System.Windows.Controls;

namespace StorAmp.Wpf.Wpf.Msg
{
   /// <summary>
   /// Interaction logic for ChannelPanel.xaml
   /// </summary>
   public partial class ChannelPanel : UserControl
   {
      public ChannelPanel()
      {
         InitializeComponent();

         StorAmp.Wpf.Wpf.AvalonSyntax.Avalon.Init();

         DataContextChanged += ChannelPanel_DataContextChanged;
      }

      private void ChannelPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {

      }
   }
}
