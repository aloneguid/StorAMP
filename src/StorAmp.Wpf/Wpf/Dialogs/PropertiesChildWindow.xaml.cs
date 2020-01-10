using System.Windows;
using System.Collections.Generic;
using MahApps.Metro.SimpleChildWindow;
using System.Collections.ObjectModel;
using Humanizer;

namespace StorAmp.Wpf.Wpf.Dialogs
{
   /// <summary>
   /// Interaction logic for MessageChildWindow.xaml
   /// </summary>
   public partial class PropertiesChildWindow : ChildWindow
   {
      public ObservableCollection<KeyValuePair<string, object>> Properties { get; } = new ObservableCollection<KeyValuePair<string, object>>();

      public PropertiesChildWindow(string title, Dictionary<string, object> properties)
      {
         InitializeComponent();

         this.Title = title;
         this.DataContext = this;

         foreach(KeyValuePair<string, object> p in properties)
         {
            Properties.Add(new KeyValuePair<string, object>(p.Key, p.Value));
         }
      }

      private void Yes_Click(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
