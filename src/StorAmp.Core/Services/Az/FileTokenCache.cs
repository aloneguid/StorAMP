using System.IO;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;

namespace StorAmp.Core.Services.Az
{
   class FileTokenCache : TokenCache
   {
      private readonly object FileLock = new object();
      private readonly string _cacheFilePath;
      private byte[] _blob;

      public FileTokenCache(string cacheFilePath)
      {
         _cacheFilePath = cacheFilePath;

         BeforeAccess = BeforeAccessImpl;
         AfterAccess = AfterAccessImpl;
      }

      private void BeforeAccessImpl(TokenCacheNotificationArgs args)
      {
         lock(FileLock)
         {
            if(_blob == null && File.Exists(_cacheFilePath))
            {
               _blob = File.ReadAllBytes(_cacheFilePath);
               args.TokenCache.Deserialize(_blob);
            }
         }
      }

      private void AfterAccessImpl(TokenCacheNotificationArgs args)
      {
         lock(FileLock)
         {
            //check byte array in memory to avoid constant disk writes
            byte[] blob = args.TokenCache.Serialize();
            if(_blob == null || !Enumerable.SequenceEqual(blob, _blob))
            {
               File.WriteAllBytes(_cacheFilePath, blob);
               _blob = blob;
            }
         }
      }
   }
}
