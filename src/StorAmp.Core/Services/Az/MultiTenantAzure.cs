using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StorAmp.Core.Model;

namespace StorAmp.Core.Services.Az
{
   using IAuthenticated = global::Microsoft.Azure.Management.Fluent.Azure.IAuthenticated;
   using Azure = global::Microsoft.Azure.Management.Fluent.Azure;

   class MultiTenantAzure : IUserInfoReceiver
   {
      private const string AzureCliClientId = "04b07795-8ddb-461a-bbee-02f9e1bf7b46";
      private readonly Dictionary<string, IAuthenticated> _tenantIdToAuthenticated = new Dictionary<string, IAuthenticated>();
      private readonly string _clientId;
      private readonly TokenCache _tokenCache;
      private bool _userInfoReceiverSet;


      public MultiTenantAzure(
         string clientId,
         TokenCache tokenCache = null)
      {
         _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
         _tokenCache = tokenCache ?? TokenCache.DefaultShared;
      }

      public static MultiTenantAzure FromAccount(ConnectedAzureAccount connectedAzureAccount)
      {
         var tokenCache = new ConnectedAzureAccountTokenCache(connectedAzureAccount);
         return new MultiTenantAzure(AzureCliClientId, tokenCache);
      }

      public IUserTokenInteraction UserTokenInteraction { get; set; }

      public UserInfo UserInfo { get; private set; }

      public async Task<IReadOnlyCollection<KeyValuePair<string, ISubscription>>> ListAllSubscriptionsAsync()
      {
         IAuthenticated globalClient = GetAuthenticated();
         IPagedCollection<ITenant> tenants = await globalClient.Tenants.ListAsync().ConfigureAwait(false);

         var all = new List<KeyValuePair<string, ISubscription>>();
         await Task.WhenAll(tenants.Select(t => ListSubscriptionsAsync(t.TenantId, all))).ConfigureAwait(false);
         return all.AsReadOnly();
      }

      private async Task ListSubscriptionsAsync(string tenantId, IList<KeyValuePair<string, ISubscription>> subscriptions)
      {
         IAuthenticated tenantClient = GetAuthenticated(tenantId);
         IPagedCollection<ISubscription> subs = await tenantClient.Subscriptions.ListAsync().ConfigureAwait(false);
         foreach(ISubscription sub in subs)
         {
            subscriptions.Add(new KeyValuePair<string, ISubscription>(tenantId, sub));
         }
      }

      public IAuthenticated GetAuthenticated(string tenantId = "common")
      {
         if(!_tenantIdToAuthenticated.TryGetValue(tenantId, out IAuthenticated auth))
         {
            var armCredentials = new CleverServiceClientCredentials(
                "https://management.azure.com/",
                _clientId,
                tenantId,
                _tokenCache);
            armCredentials.UserTokenInteraction = UserTokenInteraction;
            if(!_userInfoReceiverSet)
            {
               armCredentials.UserInfoReceiver = this;
               _userInfoReceiverSet = true;
            }

            var azureCreds = new AzureCredentials(armCredentials, null, tenantId, AzureEnvironment.AzureGlobalCloud);
            IAuthenticated authenticated = Azure.Authenticate(azureCreds);
            _tenantIdToAuthenticated[tenantId] = authenticated;
            return authenticated;
         }

         return auth;
      }

      public void SetUserInfo(UserInfo userInfo)
      {
         UserInfo = userInfo;
      }
   }

}
