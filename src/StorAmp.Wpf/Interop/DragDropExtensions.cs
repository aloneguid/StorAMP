using System.Collections.Generic;
using System.Windows;
using NetBox.Extensions;
using StorAmp.Core.Model;

namespace StorAmp.Wpf.Interop
{
   static class DragDropExtensions
   {
      public const string AccountDataFormat = "StorAmpAccount";
      public const string PropertiesDataFormat = "Properties";

      public static DragData ToDragData(this DragEventArgs e)
      {
         return (DragData) e.Data.GetData(typeof(DragData).FullName);
      }
   }
}
