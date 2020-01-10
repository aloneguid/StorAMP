using System;
using System.IO;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;
using StorAmp.Core.Services;

namespace StorAmp.Wpf.Wpf.Viewers
{
   /// <summary>
   /// Interaction logic for TextViewer.xaml
   /// </summary>
   public partial class TextViewer : UserControl, IFileEditor
   {
      public event Action<bool> FileChanged;
      private FileFormattingService _ffs;

      public TextViewer()
      {
         InitializeComponent();
      }

      public void SaveContentToFile(string filePath)
      {
         File.WriteAllText(filePath, Avalon.Text);
      }

      public void SetContentFromFile(string filePath, string formatHint)
      {
         IHighlightingDefinition highlighter =
            HighlightingManager.Instance.GetDefinitionByExtension(formatHint ?? ".txt") ??
            HighlightingManager.Instance.GetDefinitionByExtension(".txt");

         Avalon.SyntaxHighlighting = highlighter;
         string content = File.ReadAllText(filePath);
         _ffs = new FileFormattingService(content);
         _ffs.Analyse();
         ReformatButton.Visibility = _ffs.CanReformat ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
         Avalon.Text = content;
      }

      private void Avalon_TextChanged(object sender, EventArgs e)
      {
         FileChanged?.Invoke(true);
      }

      private void ReformatButton_Click(object sender, System.Windows.RoutedEventArgs e)
      {
         if(!_ffs.CanReformat)
            return;

         Avalon.Text = _ffs.FormattedContents;
      }

      public void Stop() => Avalon.Text = string.Empty;
   }
}
