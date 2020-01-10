using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using StorAmp.Core.Services.Az;

namespace StorAmp.Core.Model
{
   public class ConnectedAzureAccount : ConnectedEntity
   {
      [JsonIgnore]
      public byte[] State { get; set; }

      [JsonProperty("token")]
      private string __jsonstate
      {
         get => State == null ? null : Convert.ToBase64String(State);
         set
         {
            if(value == null)
            {
               State = null;
            }
            else
            {
               State = Convert.FromBase64String(value);
            }
         }
      }

      public TokenCredential AsTokenCredential()
      {
         return new AccountTokenCredential(this);
      }

      class AccountTokenCredential : TokenCredential
      {
         private readonly TokenCache _cache;

         public AccountTokenCredential(ConnectedAzureAccount connectedAzureAccount)
         {
            _cache = new ConnectedAzureAccountTokenCache(connectedAzureAccount);
         }

         public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
         {
            return new AccessToken();
         }

         public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
         {
            TokenCacheItem item = _cache.ReadItems().FirstOrDefault();

            return new AccessToken(item.AccessToken, item.ExpiresOn);
         }
      }
   }
}
