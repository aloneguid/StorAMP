using StackExchange.Redis;

namespace StorAmp.Core.ViewModel.Redis
{
   public class RedisHashEntryViewModel
   {
      public RedisHashEntryViewModel(HashEntry hashEntry)
      {
         Name = hashEntry.Name.ToString();
         Value = hashEntry.Value.ToString();
      }

      public string Name { get; }
      public string Value { get; }
   }
}
