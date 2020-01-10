using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using StorAmp.Core;
using Humanizer;
using Storage.Net.Blobs;
using System.Linq;
using StorAmp.Core.Model;

namespace StorAmp.Wpf.Converters
{
   public class GenericConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         bool negate = parameter is string s && s == "-1";

         switch(value)
         {
            case bool b:
               if(targetType == typeof(Visibility))
                  return negate
                     ? (b ? Visibility.Collapsed : Visibility.Visible)
                     : (b ? Visibility.Visible : Visibility.Collapsed);
               else if(targetType == typeof(bool))
                  return negate ? !b : b;
               break;

            case string str:
               if(targetType == typeof(Brush))
               {
                  return new SolidColorBrush((Color)ColorConverter.ConvertFromString(str));
               }
               else if(targetType == typeof(ImageSource))
               {
                  return null;
               }
               else if(targetType == typeof(Visibility))
               {
                  return negate
                     ? str == null ? Visibility.Visible : Visibility.Collapsed
                     : str == null ? Visibility.Collapsed : Visibility.Visible;
               }
               break;
            case DateTime dt:
               return dt.ToString();
            case DateTimeOffset dto:
               if(targetType == typeof(string))
               {
                  if(GlobalSettings.Default.HumanisedBlobChangeDates)
                     return dto.Humanize();

                  return dto.LocalDateTime.ToString();
               }
               break;
            case int i:
               if(i == 0)
                  return string.Empty;
               return i.ToString();
            case Dictionary<string, string> dic:
               if(targetType == typeof(Visibility))
               {
                  return negate
                     ? dic.Count == 0 ? Visibility.Visible : Visibility.Collapsed
                     : dic.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
               }
               if(dic.Count == 0)
                  return null;
               return dic.Select(i => new EditableKeyValue { Key = i.Key, Value = i.Value }).ToList();
            default:
               if(targetType == typeof(Visibility))
               {
                  if(parameter is Type pt)
                  {
                     return pt.IsAssignableFrom(value.GetType())
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                  }
                  else
                  {
                     return negate
                        ? value == null ? Visibility.Visible : Visibility.Collapsed
                        : value == null ? Visibility.Collapsed : Visibility.Visible;
                  }
               }
               break;
         }

         return null;
      }
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
