using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace StorAmp.Wpf.MarkupExtensions
{
   class XamlImageExtension : MarkupExtension
   {
      private readonly string _name;

      public XamlImageExtension(string name)
      {
         _name = name;
      }

      public override object ProvideValue(IServiceProvider serviceProvider)
      {
         return _name.GetImageSource();
      }
   }
}
