using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace StorAmp.Core.Model.AppInsights
{
   public class QueryResult
   {
      [JsonProperty("tables")]
      public Table[] Tables { get; set; }
   }

   public class Table
   {
      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("columns")]
      public TableColumn[] Columns { get; set; }

      [JsonProperty("rows")]
      public object[][] Rows { get; set; }
   }

   public class TableColumn
   {
      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("type")]
      public string Type { get; set; }
   }
}
