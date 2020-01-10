using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using StorAmp.Core.Services;
using StorAmp.Wpf.Wpf.Dialogs;

namespace CloudExplorer.Wpf
{
   public partial class MainWindow : IDialogService
   {
      public async Task<string> AskStringInputAsync(string title, string message, string initialValue = null)
      {
         //see https://github.com/MahApps/MahApps.Metro/blob/9955cc2a4c54c58bfdee794d5bdea57bbbfd1af3/src/MahApps.Metro.Samples/MahApps.Metro.Demo/MainWindow.xaml.cs

         string result = await this.ShowInputAsync(title, message, new MetroDialogSettings { DefaultText = initialValue });

         return result;
      }

      public async Task<bool> AskYesNoAsync(string title, string message)
      {
         var window = new MessageChildWindow(title, message) { IsModal = true };
         await this.ShowChildWindowAsync(window);
         return window.Confirmed;
      }

      public async Task ShowMessageAsync(string title, string message)
      {
         await this.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative);
      }

      public async Task ShowPropertiesAsync(string title, Dictionary<string, object> properties)
      {
         var window = new PropertiesChildWindow(title, properties) { IsModal = true };
         await this.ShowChildWindowAsync(window);
      }

      public async Task ShowDialogAsync(string title, object content, string confirmButtonText, object confirmCommand)
      {
         var window = new ContainerChildWindow(title, (UIElement)content, confirmButtonText, (ICommand)confirmCommand)
         {
            IsModal = true,
            EnableDropShadow = true,
            AllowMove = true
         };
         await this.ShowChildWindowAsync(window);
      }

      public IReadOnlyCollection<string> AskLocalFile(string title)
      {
         var dialog = new Microsoft.Win32.OpenFileDialog();
         dialog.Multiselect = true;
         dialog.Title = title;

         if(dialog.ShowDialog() == true)
         {
            return dialog.FileNames;
         }

         return null;
      }

      public string AskLocalFolder(string title)
      {
         var dialog = new FolderBrowserDialog()
         {
            AutoUpgradeEnabled = true,
            Description = title,
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
         };

         if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
         {
            return dialog.SelectedPath;
         }

         return null;
      }
   }
}
