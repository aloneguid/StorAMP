using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace StorAmp.Core.Logging
{
   public class CustomTelemetryConverter : TelemetryConverterBase
   {
      public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
      {
         if(logEvent.Exception != null)
         {
            yield return ToExceptionTelemetry(logEvent, formatProvider);
         }
         else
         {
            if(logEvent.Properties.TryGetValue("EventName", out LogEventPropertyValue eventNameValue))
            {
               string eventName = eventNameValue.ToString().Trim('"');

               logEvent.RemovePropertyIfPresent("EventName");

               var telemetry = new EventTelemetry(eventName)
               {
                  Timestamp = logEvent.Timestamp
               };

               // write logEvent's .Properties to the AI one
               ForwardEventPropertiesToTelemetryProperties(logEvent, telemetry, formatProvider);

               yield return telemetry;
            }
            else
            {
               string renderedMessage = logEvent.RenderMessage(formatProvider);

               var telemetry = new TraceTelemetry(renderedMessage)
               {
                  Timestamp = logEvent.Timestamp,
                  SeverityLevel = ToSeverityLevel(logEvent.Level)
               };

               // write logEvent's .Properties to the AI one
               ForwardPropertiesToTelemetryProperties(logEvent, telemetry, formatProvider);

               yield return telemetry;
            }
         }

      }

      private void ForwardEventPropertiesToTelemetryProperties(LogEvent logEvent, ISupportProperties telemetryProperties, IFormatProvider formatProvider)
      {
         ForwardPropertiesToTelemetryProperties(logEvent, telemetryProperties, formatProvider,
             includeLogLevel: false,
             includeRenderedMessage: false,
             includeMessageTemplate: false);
      }
   }
}
