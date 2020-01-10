using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using StorAmp.Core.Model;
using StorAmp.Core.ViewModel;
using StorAmp.Wpf.Controls;

namespace StorAmp.Wpf.Converters
{
   public class HierarchicalResourceConverter : IValueConverter
   {
      private readonly StorageTypeIconConverter _stic = new StorageTypeIconConverter();

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if(targetType == typeof(ImageSource))
         {
            if(value is HierarchicalResource hr)
            {
               if(hr.Tag is ConnectedAccount ca)
               {
                  return _stic.Convert(hr.Tag, targetType, parameter, culture);
               }

               return hr.IconPath.GetImageSource();
            }
         }
         else if(value is ObservableCollection<HierarchicalResourceCommandGroup> groups)
         {
            var result = new List<object>();

            foreach(HierarchicalResourceCommandGroup g in groups)
            {
               if(result.Count > 0)
                  result.Add(new Separator());

               foreach(HierarchicalResourceCommand cmd in g.Commands)
               {
                  result.Add(new MenuItem
                  {
                     Header = cmd.DisplayName,
                     Icon = new WinUISymbol
                     {
                        Symbol = cmd.IconSymbol,
                        HorizontalAlignment = HorizontalAlignment.Center
                     },
                     Command = cmd.Command,
                     CommandParameter = cmd.Parameter
                  });
               }
            }

            return result;
         }

         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
   }
}
