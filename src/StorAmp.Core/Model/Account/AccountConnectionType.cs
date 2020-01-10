using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorAmp.Core.Model.Account
{
   public class AccountConnectionType
   {
      public AccountConnectionType(string token, string displayName, params AccountField[] fields)
      {
         Token = token;
         DisplayName = displayName;
         Fields = fields.ToList();
      }

      public string Token { get; }

      public string DisplayName { get; }

      public IReadOnlyCollection<AccountField> Fields { get; set; }
   }
}
