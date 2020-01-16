using Storage.Net.ConnectionString;
using StorAmp.Core.Model;
using StorAmp.Core.Model.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorAmp.Core.ViewModel.Redis
{
   public class RedisAccountDefinition : AccountDefinition
   {
      public RedisAccountDefinition() : base("redis", "redis", "Redis",
         new AccountConnectionType("local", "Local",
            new AccountField("host", "Host"),
            new AccountField("port", "Port (optional)")))
      {
      }

      public override async Task<Exception> TestAndGetErrorAsync(string accountId, StorageConnectionString connectionString)
      {
         try
         {
            var vm = new RedisViewModel(new ConnectedAccount(connectionString.ToString()));

            vm.Mx.GetEndPoints();
         }
         catch(Exception ex)
         {
            return ex;
         }

         return null;
      }
   }
}
