using System.Windows;
using System.Windows.Input;
using MahApps.Metro.SimpleChildWindow;

namespace StorAmp.Wpf.Wpf.Dialogs
{
   /// <summary>
   /// Interaction logic for MessageChildWindow.xaml
   /// </summary>
   public partial class ContainerChildWindow : ChildWindow
   {
      public ContainerChildWindow(string title, UIElement content, string confirmButtonText = null, ICommand confirmCommand = null)
      {
         if(title is null)
            throw new System.ArgumentNullException(nameof(title));
         if(content is null)
            throw new System.ArgumentNullException(nameof(content));

         InitializeComponent();

         this.Title = title;
         DataContext = this;

         ConfirmButton.Visibility = confirmButtonText == null ? Visibility.Collapsed : Visibility.Visible;
         if(confirmButtonText != null)
            ConfirmButton.Content = confirmButtonText;
         if(confirmCommand != null)
            ConfirmButton.Command = confirmCommand;

         Content.Children.Clear();
         Content.Children.Add(content);
      }

      public bool Confirmed { get; set; }

      private void Confirm_Click(object sender, RoutedEventArgs e)
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
