using System;
using System.Globalization;
using System.Windows.Data;
using ICSharpCode.AvalonEdit.Highlighting;
using StorAmp.Wpf.Wpf.AvalonSyntax;

namespace StorAmp.Wpf.Converters
{
   public class ExtensionToAvalonHighlightingConverter : IValueConverter
   {
      public ExtensionToAvalonHighlightingConverter()
      {
         Avalon.Init();
      }

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(!(value is string ext))
            return null;

         IHighlightingDefinition highlighter =
            HighlightingManager.Instance.GetDefinitionByExtension(ext) ??
            HighlightingManager.Instance.GetDefinitionByExtension(".txt");

         return highlighter;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
