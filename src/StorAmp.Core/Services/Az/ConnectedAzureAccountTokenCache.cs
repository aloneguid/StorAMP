using System.Linq;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StorAmp.Core.Model;

namespace StorAmp.Core.Services.Az
{
   class ConnectedAzureAccountTokenCache : TokenCache
   {
      private readonly ConnectedAzureAccount _account;
      private static readonly object _lock = new object();

      public ConnectedAzureAccountTokenCache(ConnectedAzureAccount account)
      {
         _account = account;

         BeforeAccess = BeforeAccessImpl;
         AfterAccess = AfterAccessImpl;
      }

      private void BeforeAccessImpl(TokenCacheNotificationArgs args)
      {
         lock(_lock)
         {
            args.TokenCache.Deserialize(_account.State);
         }
      }

      private void AfterAccessImpl(TokenCacheNotificationArgs args)
      {
         lock(_lock)
         {
            byte[] blob = args.TokenCache.Serialize();
            _account.State = blob;
         }
      }
   }
}
