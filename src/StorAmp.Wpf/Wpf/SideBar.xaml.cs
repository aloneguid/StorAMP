using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetBox.Extensions;
using StorAmp.Core;
using StorAmp.Core.Model;
using StorAmp.Core.ViewModel;
using StorAmp.Core.ViewModel.Account;

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for SideBar.xaml
   /// </summary>
   public partial class SideBar : UserControl
   {
      public event Action<ConnectedAccount> ConnectedAccountDoubleTapped;

      public SideBar()
      {
         InitializeComponent();

         DataContext = this;
      }

      public SideBarViewModel ViewModel { get; } = new SideBarViewModel();

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

      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         ViewModel.InitialiseAsync().Forget();
      }

      private void Tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if(Tree.SelectedItem == null)
            return;

         object source = Tree.SelectedItem;
         if(source is HierarchicalResource hr)
            source = hr.GetDoubleTapResult();

         if(source is ConnectedAccount ca)
         {
            ConnectedAccountDoubleTapped?.Invoke(ca);
         }
      }

      private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
      {
         if(Tree.SelectedItem is ConnectedFolderViewModel folder)
         {
            GlobalState.LastConnectedFolder = folder.Folder;
         }
      }
   }
}
