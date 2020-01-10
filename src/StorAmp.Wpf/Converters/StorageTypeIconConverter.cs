using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using StorAmp.Core.Model;
using NetBox.Extensions;
using StorAmp.Core.Model.Account;
using Storage.Net;

namespace StorAmp.Wpf.Converters
{
   public class StorageTypeIconConverter : IValueConverter
   {
      private static readonly Dictionary<string, ImageSource> _prefixToBitmap = new Dictionary<string, ImageSource>();

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         string prefix = null;

         switch(value)
         {
            case AccountDefinition ad:
               prefix = ad.Prefix;
               break;
            case ConnectedAccount ca:
               prefix = ca.Prefix;
               break;
         }

         if(prefix == null)
            return null;

         return _prefixToBitmap.GetOrAdd(prefix, () => $"account/{prefix}".GetImageSource());
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
   }
}
