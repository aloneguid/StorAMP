using System;
using StorAmp.Core.Tasks;

namespace StorAmp.Core.Model.Messages
{
   public class BackgroundTaskMessage
   {
      public BackgroundTaskMessage(BackgroundTask backgroundTask)
      {
         BackgroundTask = backgroundTask ?? throw new ArgumentNullException(nameof(backgroundTask));
      }

      public BackgroundTask BackgroundTask { get; }
   }
}
