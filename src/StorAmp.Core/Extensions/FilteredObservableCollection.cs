using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace StorAmp.Core.Extensions
{
   /// <summary>
   /// Maintains a copy of an observable collection, but filters the content to only one that is important
   /// </summary>
   /// <typeparam name="T"></typeparam>
   class FilteredObservableCollection<T> : ObservableCollection<T>
   {
      private readonly Func<T, bool> _isValidItem;

      public FilteredObservableCollection(ObservableCollection<T> parent, Func<T, bool> isValidItem)
      {
         if(parent is null)
            throw new ArgumentNullException(nameof(parent));
         _isValidItem = isValidItem ?? throw new ArgumentNullException(nameof(isValidItem));

         parent.CollectionChanged += OnCollectionChanged;
      }

      private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         switch(e.Action)
         {
            case NotifyCollectionChangedAction.Add:
               foreach(T item in e.NewItems)
               {
                  if(_isValidItem(item))
                     Add(item);
               }
               break;
            case NotifyCollectionChangedAction.Remove:
               foreach(T item in e.OldItems)
               {
                  if(Contains(item))
                     Remove(item);
               }
               break;
            default:
               throw new NotImplementedException();
         }
      }
   }
}
