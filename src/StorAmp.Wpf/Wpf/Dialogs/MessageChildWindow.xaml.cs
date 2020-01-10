using System.Windows;
using MahApps.Metro.SimpleChildWindow;

namespace StorAmp.Wpf.Wpf.Dialogs
{
   /// <summary>
   /// Interaction logic for MessageChildWindow.xaml
   /// </summary>
   public partial class MessageChildWindow : ChildWindow
   {
      public MessageChildWindow(string title, string message)
      {
         InitializeComponent();

         this.Title = title;
         Message = message;
         DataContext = this;
      }

      public string Message { get; }

      public bool Confirmed { get; set; }

      private void Yes_Click(object sender, RoutedEventArgs e)
      {
         Confirmed = true;
         Close();
      }

      private void No_Click(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
