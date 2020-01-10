using System;
using System.Windows.Controls;

namespace StorAmp.Wpf.Wpf.Viewers
{
   /// <summary>
   /// Interaction logic for VideoViewer.xaml
   /// </summary>
   public partial class VideoViewer : UserControl, IFileViewer
   {
      public VideoViewer()
      {
         InitializeComponent();
      }

      public void SetContentFromFile(string filePath, string formatHint)
      {
         MediaElement.Source = new Uri(filePath);
      }

      public void Stop()
      {
         MediaElement.Source = null;
      }
   }
}
