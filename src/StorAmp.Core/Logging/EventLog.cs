using Serilog.Context;

namespace Serilog
{
   public static class EventLog
   {
      public static void LogEvent(string eventName, string messageTemplate, params object[] propertyValues)
      {
         using(LogContext.PushProperty("EventName", eventName))
         {
            Log.Information(messageTemplate, propertyValues);
         }
      }
   }
}
