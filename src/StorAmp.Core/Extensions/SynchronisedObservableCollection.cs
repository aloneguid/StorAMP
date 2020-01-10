using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace StorAmp.Core.Extensions
{
   class SynchronisedObservableCollection<TThisElement, TSourceElement> : ObservableCollection<TThisElement>
   {
      private readonly Func<TSourceElement, TThisElement> _toThisElement;
      private readonly Dictionary<TSourceElement, TThisElement> _sourceToThis = new Dictionary<TSourceElement, TThisElement>();

      public SynchronisedObservableCollection(
         ObservableCollection<TSourceElement> sourceCollection,
         Func<TSourceElement, TThisElement> toThisElement)
      {
         if(sourceCollection is null)
            throw new ArgumentNullException(nameof(sourceCollection));

         _toThisElement = toThisElement ?? throw new ArgumentNullException(nameof(toThisElement));

         //transfer all elements
         foreach(TSourceElement src in sourceCollection)
         {
            AddInternal(src);
         }

         //subscribe to notifications
         sourceCollection.CollectionChanged += SourceCollection_CollectionChanged;
      }

      private void AddInternal(TSourceElement element)
      {
         TThisElement dest = _toThisElement(element);
         _sourceToThis[element] = dest;
         Add(dest);
      }

      private void SourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         switch(e.Action)
         {
            case NotifyCollectionChangedAction.Add:
               foreach(TSourceElement src in e.NewItems)
               {
                  AddInternal(src);
               }
               break;
            case NotifyCollectionChangedAction.Remove:
               foreach(TSourceElement src in e.OldItems)
               {
                  if(_sourceToThis.TryGetValue(src, out TThisElement dest))
                  {
                     Remove(dest);
                     _sourceToThis.Remove(src);
                  }
               }
               break;
            default:
               throw new NotImplementedException();
         }
      }
   }
}
