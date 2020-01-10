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

         DataContextChanged += ChannelPanel_DataContextChanged;
      }

      private void ChannelPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {

      }
   }
}
