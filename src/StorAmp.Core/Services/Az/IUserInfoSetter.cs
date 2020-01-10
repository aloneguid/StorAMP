using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace StorAmp.Core.Services.Az
{
   interface IUserInfoReceiver
   {
      void SetUserInfo(UserInfo userInfo);
   }
}
