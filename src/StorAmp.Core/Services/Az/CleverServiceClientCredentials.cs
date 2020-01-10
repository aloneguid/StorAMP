using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace StorAmp.Core.Services.Az
{
   class CleverServiceClientCredentials : ServiceClientCredentials
   {
      private readonly AuthenticationContext _authenticationContext;
      private readonly string _audience;
      private readonly string _clientId;

      public CleverServiceClientCredentials(string audience, string clientId, string tenantId = "common", TokenCache tokenCache = null)
      {
         _authenticationContext = new AuthenticationContext(
             "https://login.windows.net/" + tenantId,
             false,
             tokenCache ?? TokenCache.DefaultShared);
         _audience = audience ?? throw new ArgumentNullException(nameof(audience));
         _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
      }

      public IUserTokenInteraction UserTokenInteraction { get; set; }

      public IUserInfoReceiver UserInfoReceiver { get; set; }

      public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
      {
         AuthenticationResult ar;
         try
         {
            ar = await _authenticationContext.AcquireTokenSilentAsync(_audience, _clientId).ConfigureAwait(false);
         }
         catch(AdalSilentTokenAcquisitionException ex)
         {
            if(UserTokenInteraction == null)
            {
               throw new AuthenticationException("Interactive logon required, but no UI element was supplied", ex);
            }

            DeviceCodeResult dc = await _authenticationContext.AcquireDeviceCodeAsync(_audience, _clientId);

            UserTokenInteraction.UserCode = dc.UserCode;
            UserTokenInteraction.VerificationUrl = dc.VerificationUrl;

            ar = await _authenticationContext.AcquireTokenByDeviceCodeAsync(dc);

            await UserTokenInteraction.CompleteAuthAsync();
         }
         catch(Exception ex)
         {
            throw ex;
         }

         if(UserInfoReceiver != null)
            UserInfoReceiver.SetUserInfo(ar.UserInfo);
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ar.AccessToken);
      }
   }

}
