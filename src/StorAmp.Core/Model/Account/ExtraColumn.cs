using System;
using Storage.Net.Blobs;

namespace StorAmp.Core.Model.Account
{
   public class ExtraColumn
   {
      public ExtraColumn(string name, string hint, Func<Blob, string> value)
      {
         Name = name;
         Hint = hint;
         Value = value;
      }

      public string Name { get; }
      public string Hint { get; }
      public Func<Blob, string> Value { get; }
   }

   public class ExtraPropertyColumn : ExtraColumn
   {
      public ExtraPropertyColumn(string name, string propertyName)
         : base(name, null, b => GetValue(b, propertyName))
      {

      }

      private static string GetValue(Blob b, string propertyName)
      {
         b.Properties.TryGetValue(propertyName, out object value);
         if(value is bool vb)
            return vb ? "✔" : "❌";
         return value?.ToString();
      }
   }

}
