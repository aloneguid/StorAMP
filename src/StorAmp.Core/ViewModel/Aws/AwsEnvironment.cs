using Newtonsoft.Json;

namespace StorAmp.Core.ViewModel.Aws
{
   public class AwsEnvironment : HierarchicalResource
   {
      public AwsEnvironment(string name) : base(name, "cloud-aws")
      {

      }

      [JsonProperty("id")]
      public string Id { get; set; }
   }
}
