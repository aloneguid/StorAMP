using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StorAmp.Core.Extensions
{
   public static class ObservableExtensions
   {

      public static void AddAll<T>(this ObservableCollection<T> collection, IEnumerable<T> items, bool clear = false)
      {
         if(clear)
            collection.Clear();

         foreach(T item in items)
         {
            collection.Add(item);
         }
      }
   }
}
