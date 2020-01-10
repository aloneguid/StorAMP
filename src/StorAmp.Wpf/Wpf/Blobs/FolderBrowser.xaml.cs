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
using StorAmp.Core.ViewModel.Blobs;

namespace StorAmp.Wpf.Wpf.Blobs
{
   /// <summary>
   /// Interaction logic for FolderBrowser.xaml
   /// </summary>
   public partial class FolderBrowser : UserControl
   {
      public FolderBrowser()
      {
         InitializeComponent();
      }

      public FolderBrowserViewModel ViewModel => (FolderBrowserViewModel)DataContext;

      private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
      {
         TreeViewItem item = sender as TreeViewItem;
         if(item != null)
         {
            item.Focus();
            item.IsSelected = true;
            //e.Handled = true;

         }
      }

      private void Tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if(Tree.SelectedItem == null)
            return;

         object source = Tree.SelectedItem;
         if(source is FolderResource hr)
         {
            ViewModel.DoubleTapFolder(hr.Blob);
         }
      }

   }
}