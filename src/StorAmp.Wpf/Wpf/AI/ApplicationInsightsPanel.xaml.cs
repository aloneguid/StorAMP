using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using StorAmp.Core.Model.AppInsights;
using StorAmp.Core.ViewModel;
using StorAmp.Wpf.Wpf.AvalonSyntax;
using Table = StorAmp.Core.Model.AppInsights.Table;
using TableColumn = StorAmp.Core.Model.AppInsights.TableColumn;

namespace StorAmp.Wpf.Wpf
{
   /// <summary>
   /// Interaction logic for ApplicationInsightsPanel.xaml
   /// </summary>
   public partial class ApplicationInsightsPanel : UserControl
   {
      private bool _avalonLocked;

      public ApplicationInsightsPanel()
      {
         InitializeComponent();

         Avalon.Init();

         QueryEditor.TextArea.TextEntered += TextArea_TextEntered;
      }

      private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
      {
         //if(e.Text)
      }

      private ApplicationInsightsPanelViewModel ViewModel => (ApplicationInsightsPanelViewModel)DataContext;

      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         if(ViewModel == null)
            return;

         ViewModel.PropertyChanged += (sender, e) =>
         {
            if(e.PropertyName == nameof(ApplicationInsightsPanelViewModel.QueryText))
            {
               if(!_avalonLocked)
               {
                  QueryEditor.Text = ViewModel.QueryText;
               }
            }
            else if(e.PropertyName == nameof(ApplicationInsightsPanelViewModel.QueryResult))
            {
               RenderResult(ViewModel.QueryResult);
            }
         };
      }

      private void RenderResult(QueryResult qr)
      {
         Grid.Items.Clear();
         Grid.Columns.Clear();

         Table table = qr.Tables[0];

         //add columns
         int iCol = 0;
         foreach(TableColumn column in table.Columns)
         {
            Grid.Columns.Add(new DataGridTextColumn
            {
               Header = column.Name,
               Binding = new Binding($"[{iCol++}]") { Mode = BindingMode.OneTime }
            });
         }

         foreach(object[] row in table.Rows)
         {
            for(int i = 0; i < table.Rows.Length; i++)
            {
               Grid.Items.Add(row);
            }
         }
      }

      private void QueryEditor_TextChanged(object sender, EventArgs e)
      {
         _avalonLocked = true;
         ViewModel.QueryText = QueryEditor.Text;
         _avalonLocked = false;
      }
   }
}
