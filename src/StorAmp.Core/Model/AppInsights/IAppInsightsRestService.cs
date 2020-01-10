using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Refit;

namespace StorAmp.Core.Model.AppInsights
{
   public class AppInsightsQuery
   {
      [JsonProperty("query")]
      public string Query { get; set; }
   }

   //for samples see https://dev.applicationinsights.io/apiexplorer/postQuery?appId=3f780631-5bb5-4cd3-b3b4-98afd81f49cc&apiKey=s6kkdl3nij811ta6rvpok2be7211cvpbyx25w0j6&body=traces

   public interface IAppInsightsRestService
   {
      [Post("/v1/apps/{appId}/query")]
      Task<QueryResult> PostQueryAsync(
         string appId,
         [Header("x-api-key")] string apiKey,
         [Body] AppInsightsQuery query);
   }
}
