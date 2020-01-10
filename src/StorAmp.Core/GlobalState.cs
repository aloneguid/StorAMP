using StorAmp.Core.Model;

namespace StorAmp.Core
{
   public static class GlobalState
   {
      private static ConnectedFolder _lastConnectedFolder;

      public static ConnectedFolder LastConnectedFolder
      {
         set { _lastConnectedFolder = value; }
         get
         {
            return _lastConnectedFolder ?? ConnectedAccount.RootFolder;
         }
      }
   }
}
