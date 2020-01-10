using System;
using System.Globalization;
using System.Windows.Data;
using Humanizer;
using Humanizer.Bytes;

namespace StorAmp.Wpf.Converters
{
   public class FileSizeConverter : IValueConverter
   {
      private const string SizeFormat = "#.##";

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(value is long l)
            return ByteSize.FromBytes(l).Humanize(SizeFormat);

         if(value is int i)
            return ByteSize.FromBytes(i).Humanize(SizeFormat);

         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
