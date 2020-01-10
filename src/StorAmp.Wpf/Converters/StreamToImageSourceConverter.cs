using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StorAmp.Wpf.Converters
{
   class StreamToImageSourceConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(!(value is Stream source))
            return null;

         return BitmapFrame.Create(source, BitmapCreateOptions.None, BitmapCacheOption.None);
      }
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
