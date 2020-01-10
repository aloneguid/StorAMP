using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using Humanizer;

namespace StorAmp.Wpf.Converters
{
   public class HumanizeNumberConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(value is long l)
         {
            if(parameter is string s)
            {
               return s.ToQuantity(l);
            }
         }

         if(value is int i)
         {
            if(parameter is string s)
            {
               return s.ToQuantity(i);
            }
         }


         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
