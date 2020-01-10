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

namespace StorAmp.Wpf.Wpf.Viewers
{
   /// <summary>
   /// Interaction logic for PictureViewer.xaml
   /// </summary>
   public partial class PictureViewer : UserControl, IFileViewer
   {
      public PictureViewer()
      {
         InitializeComponent();
      }

      public void SetContentFromFile(string filePath, string formatHint)
      {
         //to prevent file locking, this needs to be initialised with begin/end init

         var bi = new BitmapImage();
         bi.BeginInit();
         bi.CacheOption = BitmapCacheOption.OnLoad;
         bi.UriSource = new Uri(filePath);
         bi.EndInit();

         Image.Source = bi;
      }

      public void Stop() => Image.Source = null;
   }
}
