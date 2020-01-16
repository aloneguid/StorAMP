using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using StackExchange.Redis;
using StorAmp.Core.Model;

namespace StorAmp.Core.ViewModel.Redis
{
   public class RedisViewModel : ViewModelBase, IAccountPaneViewModel
   {
      private ConnectionMultiplexer _mx;
      private IDatabase _db;

      public ObservableCollection<RedisKeyViewModel> Keys { get; set; } = new ObservableCollection<RedisKeyViewModel>();

      public RedisViewModel()
      {

      }

      public RedisViewModel(ConnectedAccount connectedAccount)
      {
         Account = connectedAccount;

         RefreshKeysAsync().Forget();
      }

      public ConnectionMultiplexer Mx
      {
         get
         {
            if(_mx == null)
            {
               string config = Account.StorageConnectionString["host"];
               string port = Account.StorageConnectionString["port"];
               if(!string.IsNullOrEmpty(port))
               {
                  config += ":" + port;
               }

               _mx = ConnectionMultiplexer.Connect(config);
            }

            return _mx;
         }
      }

      private IDatabase Db => _db ?? (_db = Mx.GetDatabase());

      public ConnectedAccount Account { get; }

      public bool AcceptsDrop(DragData data) => false;
      public void DropDataAsync(DragData data) { }
      public string GetSettings() => null;
      public void RestoreSettings(string settings) { }

      private RelayCommand _RefreshKeysCommand;
      public RelayCommand RefreshKeysCommand => _RefreshKeysCommand ??= new RelayCommand(() => RefreshKeysAsync().Forget());

      public async Task RefreshKeysAsync()
      {
         Keys.Clear();

         EndPoint[] endpoints = Mx.GetEndPoints();

         foreach(EndPoint ep in endpoints)
         {
            IServer server = Mx.GetServer(ep);
            IEnumerable<RedisKey> keys = server.Keys();
            foreach(RedisKey key in keys)
            {
               RedisType keyType = await Db.KeyTypeAsync(key);

               Keys.Add(new RedisKeyViewModel(key.ToString(), keyType));
            }
         }
      }

      private RelayCommand _RefreshSelectedValueCommand;
      public RelayCommand RefreshSelectedValueCommand => _RefreshSelectedValueCommand ??= new RelayCommand(() => RefreshSelectedValueAsync().Forget());

      public async Task RefreshSelectedValueAsync()
      {
         StringValue = null;
         HashValue = null;

         if(SelectedKey == null) return;

         switch(SelectedKey.Type)
         {
            case RedisType.String:
               RedisValue stringValue = await Db.StringGetAsync(SelectedKey.Key);
               StringValue = stringValue.ToString();
               break;
            case RedisType.Hash:
               HashEntry[] hashEntries = Db.HashGetAll(SelectedKey.Key);
               HashValue = new RedisHashViewModel(SelectedKey, hashEntries);
               break;
         }
         
      }

      private RedisKeyViewModel _SelectedKey;
      public RedisKeyViewModel SelectedKey
      {
         get => _SelectedKey;
         set { Set(() => SelectedKey, ref _SelectedKey, value); RefreshSelectedValueAsync().Forget(); }
      }


      private string _StringValue;
      public string StringValue
      {
         get => _StringValue;
         set { Set(() => StringValue, ref _StringValue, value); }
      }


      private RedisHashViewModel _HashValue;
      public RedisHashViewModel HashValue
      {
         get => _HashValue;
         set { Set(() => HashValue, ref _HashValue, value); }
      }

   }
}
