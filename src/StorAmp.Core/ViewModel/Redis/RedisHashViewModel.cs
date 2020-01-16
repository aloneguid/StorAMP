using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using GalaSoft.MvvmLight;
using StackExchange.Redis;

namespace StorAmp.Core.ViewModel.Redis
{
   public class RedisHashViewModel : ViewModelBase
   {
      public RedisHashViewModel(RedisKeyViewModel key, IEnumerable<HashEntry> entries)
      {
         Key = key;
         
         foreach(HashEntry entry in entries)
         {
            Entries.Add(new RedisHashEntryViewModel(entry));
         }
      }

      public RedisKeyViewModel Key { get; }

      public ObservableCollection<RedisHashEntryViewModel> Entries { get; set; } = new ObservableCollection<RedisHashEntryViewModel>();
   }
}
