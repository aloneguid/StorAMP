using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StorAmp.Core.ViewModel;
using Storage.Net.Blobs;
using System.Diagnostics;
using System;
using System.Windows.Data;
using StorAmp.Core.Model.Account;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using StorAmp.Wpf.Controls;
using StorAmp.Wpf.Wpf.Blobs;
using System.Windows.Controls.Primitives;
using NetBox.Extensions;
using System.Windows.Media;
using StorAmp.Core;

namespace CloudExplorer.Wpf.Wpf
{
   public partial class BlobListPanel : UserControl
   {
      private readonly int _blobContextMenuItemCount;
      private readonly HashSet<DataGridColumn> _extraColumns = new HashSet<DataGridColumn>();

      public BlobListPanel()
      {
         InitializeComponent();

         _blobContextMenuItemCount = BlobContextMenu.Items.Count;

         DataContextChanged += BlobListPanel_DataContextChanged;
      }

      private void BlobListPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
         if(ViewModel != null)
         {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            ViewModel.BlobsReloaded += ViewModel_BlobsReloaded;

            if(ViewModel.Blobs.Count > 0)
            {
               ViewModel_BlobsReloaded(0);
               ViewModel.RefreshBlobsAsync().Forget();
            }
         }
      }

      private ExtraColumnView _columnView;

      private void ViewModel_BlobsReloaded(int obj)
      {
         if(!(ViewModel.Account.Definition is BlobApplicationAccount blobAccount))
            return;

         if(GlobalSettings.Default.AlternateRowColours)
         {
            ItemsGrid.AlternatingRowBackground = (Brush)FindResource("MahApps.Brushes.Accent4");
            ItemsGrid.AlternationCount = 2;
         }
         else
         {
            ItemsGrid.AlternationCount = 0;
            ItemsGrid.AlternatingRowBackground = null;
         }

         ExtraColumnView columnView = blobAccount.MatchColumnView(ViewModel.Blobs.FirstOrDefault());
         if(columnView == _columnView)
            return;

         _columnView = columnView;

         //remove all extra columns
         for(int i = ItemsGrid.Columns.Count - 1; i >=0; i--)
         {
            DataGridColumn dgc = ItemsGrid.Columns[i];

            if(_extraColumns.Contains(dgc))
            {
               ItemsGrid.Columns.RemoveAt(i);
               _extraColumns.Remove(dgc);
            }
         }

         int insertIdx = 1;   //after the first column
         foreach(string columnName in columnView.ColumnNames)
         {
            var column = new DataGridTextColumn
            {
               //Header = new DataGridRowHeader { Content = columnName, ToolTip = columnView.GetTooltip(columnName) },
               Header = new TextBlock {  Text = columnName.ToUpper(), ToolTip = columnView.GetTooltip(columnName) },
               Width = DataGridLength.Auto,
               Binding = new Binding
               {
                  Mode = BindingMode.OneTime,
                  Converter = new ExtraColumnValueConverter(columnView.GetEvaluator(columnName))
               }
            };
            _extraColumns.Add(column);

            ItemsGrid.Columns.Insert(insertIdx++, column);
         }
      }

      private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if(e.PropertyName == nameof(ViewModel.SelectedBlob))
         {
            if(ViewModel.Account.Definition is BlobApplicationAccount baa)
            {
               UiBuilder.BuildMenu(BlobContextMenu, _blobContextMenuItemCount, baa, ViewModel.Storage, ViewModel.SelectedBlob);
            }
         }
      }

      private BlobStoragePanelViewModel ViewModel => (BlobStoragePanelViewModel)DataContext;

      class ExtraColumnValueConverter : IValueConverter
      {
         private readonly Func<Blob, string> _calculator;

         public ExtraColumnValueConverter(Func<Blob, string> calculator)
         {
            _calculator = calculator;
         }

         public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         {
            if(value is Blob blob)
            {
               return _calculator(blob);
            }

            return null;
         }

         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
      }

      private void ItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         //due to the fact that DataGrid doesn't bind to multi-selected rows, this is a hack

         foreach(Blob item in e.AddedItems)
         {
            if(item.Name != BlobStoragePanelViewModel.ParentFolderName)
            {
               ViewModel.SelectedBlobs.Add(item);
            }
         }

         foreach(Blob item in e.RemovedItems)
         {
            ViewModel.SelectedBlobs.Remove(item);
         }
      }

      #region [ Drag & Drop Customisations ]

      //https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/drag-and-drop-overview#implementing-drag-and-drop

      /*private void ItemsGrid_MouseMove(object sender, MouseEventArgs e)
      {
         if(e.LeftButton == MouseButtonState.Released)
         {
            _dragDataInOperation = null;
            return;
         }

         DragData dobj = _dragDataInOperation ?? (_dragDataInOperation = CreateDragDropObject(e));
         if(dobj != null)
         {
            DragDrop.DoDragDrop(this, dobj, DragDropEffects.Copy);
         }
      }

      private DragData CreateDragDropObject(MouseEventArgs e)
      {
         return new DragData
         {
            Account = ViewModel.Account,
            Properties = new Dictionary<string, object>
            {
               ["blobs"] = ViewModel.SelectedBlobs.ToList(),
               ["storage"] = ViewModel.Storage
            }
         };
      }*/

      private async void ItemsGrid_Drop(object sender, DragEventArgs e)
      {
         if(e.Data.GetDataPresent(DataFormats.FileDrop))
         {
            object rawFileData = e.Data.GetData(DataFormats.FileDrop);
            if(rawFileData is string[] filePaths)
            {
               await ViewModel.UploadLocalFilesAsync(filePaths);
            }
         }
      }

      private void ItemsGrid_DragOver(object sender, DragEventArgs e)
      {
         //only accept file drops, everything else goes via viewmodel
         if(e.Data.GetDataPresent(DataFormats.FileDrop))
         {
            e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            Debug.WriteLine("drag over (file)");
         }
         else
         {
            e.Effects = DragDropEffects.None;
            Debug.WriteLine("drag over => none");
         }
         e.Handled = true;
      }
      #endregion

      #region [ Keyboard & Mouse Intercepts ]

      private async void ItemsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
      {
         //by default it moves to the next row
         if(e.Key == Key.Return)
         {
            if(ItemsGrid.SelectedItem is Blob blob)
               await ViewModel.ItemActionAsync(blob);

            e.Handled = true;            
         }
      }

      private async void ItemsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
      {
         if(ItemsGrid.SelectedItem is Blob blob)
            await ViewModel.ItemActionAsync(blob);
      }

      #endregion

      private void ItemsGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
         if(ViewModel == null)
            return;

         ViewModel.BlobsReloaded += (count) =>
         {
            ItemsGrid.SelectedIndex = 0;

            //firstRow is null here, because blobs are not rendered to containers yet
            /*var firstRow = (DataGridRow)ItemsGrid.ItemContainerGenerator.ContainerFromIndex(0);
            FocusManager.SetIsFocusScope(firstRow, true);
            FocusManager.SetFocusedElement(firstRow, firstRow);*/
         };
      }

      private void TextBox_KeyUp_UpdateBinding(object sender, KeyEventArgs e)
      {
         //update binding on enter pressed
         if(e.Key == Key.Enter)
         {
            TextBox tBox = (TextBox)sender;
            DependencyProperty prop = TextBox.TextProperty;

            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
            if(binding != null)
            {
               binding.UpdateSource();
            }
         }
      }
   }
}
