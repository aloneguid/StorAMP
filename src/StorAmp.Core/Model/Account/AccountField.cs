using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NetBox.Extensions;

namespace StorAmp.Core.Model.Account
{
   public class HiddenAccountField : AccountField
   {
      private readonly string _constantValue;

      public HiddenAccountField(string name, string constantValue) : base(name, string.Empty)
      {
         _constantValue = constantValue;
      }

      public override string Value { get => _constantValue; set { } }
   }

   public class SensitiveAccountField : AccountField
   {
      public SensitiveAccountField(string name, string displayName) : base(name, displayName)
      {
      }
   }

   public class DropDownAccountField : AccountField
   {
      public ObservableCollection<string> Values { get; set; } = new ObservableCollection<string>();

      public DropDownAccountField(string name, string displayName, string commaSeparatedValues) : base(name, displayName)
      {
         foreach(string value in commaSeparatedValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries ))
         {
            Values.Add(value);
         }
      }
   }

   public class TextAreaAccountField : AccountField
   {
      public TextAreaAccountField(string name, string displayName, bool base64Encode) : base(name, displayName)
      {
         Base64Encode = base64Encode;
      }

      public bool Base64Encode { get; }

      public string DisplayValue
      {
         get => string.IsNullOrEmpty(Value) ? string.Empty : base.Value.Base64Decode();
         set
         {
            string bv = string.IsNullOrEmpty(value) ? string.Empty : value.Base64Encode();

            Value = bv;
         }
      }
   }

   public class AccountField : INotifyPropertyChanged
   {
      private string _displayName;
      private string _value;
      private string _name;

      public event PropertyChangedEventHandler PropertyChanged;

      public AccountField(string name, string displayName)
      {
         _name = name;
         _displayName = displayName;
      }

      public string Name
      {
         get => _name;
         set
         {
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
         }
      }

      public string DisplayName
      {
         get => _displayName;
         set
         {
            _displayName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
         }
      }

      public virtual string Value
      {
         get => _value;
         set
         {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
         }
      }

      
   }
}
