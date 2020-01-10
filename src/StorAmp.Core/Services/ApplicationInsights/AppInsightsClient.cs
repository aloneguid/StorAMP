using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;
using StorAmp.Core.Model.AppInsights;

namespace StorAmp.Core.Services.ApplicationInsights
{
   public class AppInsightsClient
   {
      private readonly string _appId;
      private readonly string _key;
      private IAppInsightsRestService _rest;

      public AppInsightsClient(string appId, string key)
      {
         _rest = RestService.For<IAppInsightsRestService>("https://api.applicationinsights.io");
         _appId = appId;
         _key = key;
      }

      public async Task<QueryResult> ExecuteAsync(string query)
      {
         return await _rest.PostQueryAsync(_appId, _key, new AppInsightsQuery { Query = query });
      }
   }
}
