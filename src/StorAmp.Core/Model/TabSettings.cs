using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetBox.Extensions;

namespace StorAmp.Core.Model
{
   public class TabSettings
   {
      public TabSettings(string accountId, bool isActive, string settings)
      {
         AccountId = accountId;
         IsActive = isActive;
         Settings = settings;
      }

      public TabSettings(string s)
      {
         string[] parts = s.Split(':');

         if(parts.Length > 0)
         {
            AccountId = parts[0];
         }

         if(parts.Length > 1)
         {
            IsActive = parts[1] == "a";
         }

         if(parts.Length > 2)
         {
            Settings = parts[2].Base64Decode();
         }
      }

      public string AccountId { get; }
      public bool IsActive { get; }
      public string Settings { get; }

      public override string ToString() => $"{AccountId}:{(IsActive ? "a" : "")}:{Settings?.Base64Encode()}";

      public static string ToString(IEnumerable<TabSettings> tabSettings)
      {
         return string.Join(";", tabSettings.Select(s => s.ToString()));
      }

      public static IReadOnlyCollection<TabSettings> FromString(string s)
      {
         var ts = new List<TabSettings>();

         if(string.IsNullOrWhiteSpace(s))
            return ts;

         foreach(string part in s.Split(';'))
         {
            ts.Add(new TabSettings(part));
         }

         return ts;
      }
   }
}
