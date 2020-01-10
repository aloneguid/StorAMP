using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StorAmp.Core.Model
{
   public class EditableKeyValue : INotifyPropertyChanged
   {
      private string _key;
      private string _value;

      public event PropertyChangedEventHandler PropertyChanged;

      public string Key
      {
         get => _key;
         set
         {
            _key = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
         }
      }

      public string Value
      {
         get => _value;
         set
         {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
         }
      }

      public static Dictionary<string, string> ToDictionary(IEnumerable<EditableKeyValue> items)
      {
         return items.ToDictionary(i => i.Key, i => i.Value);
      }
   }

}
