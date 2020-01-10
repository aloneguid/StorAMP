using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using CsvHelper;

namespace StorAmp.Wpf.Wpf.Viewers
{
   /// <summary>
   /// Interaction logic for CsvViewer.xaml
   /// </summary>
   public partial class CsvViewer : UserControl, IFileViewer
   {
      public CsvViewer()
      {
         InitializeComponent();
      }

      public void SetContentFromFile(string filePath, string formatHint)
      {
         using(var reader = new StreamReader(filePath))
         {
            using(var csv = new CsvReader(reader))
            {
               using(var dr = new CsvDataReader(csv))
               {
                  var dt = new DataTable();
                  dt.Load(dr);

                  CsvGrid.ItemsSource = dt.AsDataView();
               }
            }
         }
      }

      public void Stop()
      {
         CsvGrid.ItemsSource = null;
      }
   }
}
