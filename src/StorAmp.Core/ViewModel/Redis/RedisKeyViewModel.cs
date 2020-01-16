using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace StorAmp.Core.ViewModel.Redis
{
   public class RedisKeyViewModel
   {
      public RedisKeyViewModel(RedisKey key, RedisType type)
      {
         Key = key;
         Name = key.ToString();
         Type = type;
      }

      public RedisKey Key { get; }

      public string Name { get; }

      public RedisType Type { get; }
   }
}
