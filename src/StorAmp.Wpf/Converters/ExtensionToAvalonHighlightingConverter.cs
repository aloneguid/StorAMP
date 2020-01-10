using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using ICSharpCode.AvalonEdit.Highlighting;

namespace StorAmp.Wpf.Converters
{
   public class ExtensionToAvalonHighlightingConverter : IValueConverter
   {
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
