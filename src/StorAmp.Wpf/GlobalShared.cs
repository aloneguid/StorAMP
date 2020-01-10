using System;
using Serilog;

namespace StorAmp.Wpf
{
   public static class GlobalShared
   {
      public static void Initialise(string appVersion, string aiKey)
      {
         Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.WithProperty("Version", appVersion)
           .Enrich.WithProperty("MachineName", Environment.MachineName)
           .Enrich.WithProperty("OsVersion", Environment.OSVersion.ToString())
           .Enrich.FromLogContext()
         #if DEBUG
            .WriteTo.Trace()
         #else
            .WriteTo.ApplicationInsights(aiKey, new CustomTelemetryConverter())
         #endif
            .CreateLogger();

         EventLog.LogEvent("app start", "application started");
      }
   }
}
