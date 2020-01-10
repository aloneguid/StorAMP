using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Storage.Net;
using Storage.Net.Messaging;
using StorAmp.Core.Model;
using StorAmp.Core.Services;

namespace StorAmp.Core.ViewModel.Msg
{
   public class MessengerViewModel : ViewModelBase, IAccountPaneViewModel
   {
      public MessengerViewModel()
      {

      }

      public MessengerViewModel(ConnectedAccount connectedAccount)
      {
         Account = connectedAccount;

         RefreshAsync().Forget();
      }

      public ObservableCollection<string> ChannelNames { get; } = new ObservableCollection<string>();

      public async Task RefreshAsync()
      {
         Messenger = StorageFactory.Messages.MessengerFromConnectionString(Account.ConnectionString);

         IReadOnlyCollection<string> channels = await Messenger.ListChannelsAsync();
         ChannelNames.Clear();
         ChannelNames.AddRange(channels);
      }

      public IMessenger Messenger { get; private set; }

      #region [ IAccountPaneViewModel ]

      public ConnectedAccount Account { get; private set; }

      public bool AcceptsDrop(DragData data) => false;

      public void DropDataAsync(DragData data) { }

      public string GetSettings() => null;
      public void RestoreSettings(string settings) { }

      #endregion

      #region [ Commands ]

      private RelayCommand _RefreshCommand;
      public RelayCommand RefreshCommand
      {
         get
         {
            return _RefreshCommand
                ?? (_RefreshCommand = new RelayCommand(
                () =>
                {
                   RefreshAsync().Forget();
                }));
         }
      }

      private RelayCommand _CreateChannelCommand;
      public RelayCommand CreateChannelCommand => _CreateChannelCommand ??= new RelayCommand(() => CreateChannelAsync().Forget());

      public async Task CreateChannelAsync()
      {
         string channelName = await ServiceLocator.Dialogs.AskStringInputAsync("New Channel", "Type channel name");

         if(!string.IsNullOrEmpty(channelName))
         {
            await Messenger.CreateChannelAsync(channelName);

            await RefreshAsync();
         }

      }

      #endregion

      #region [ Properties ]


      private string _SelectedChannelName;
      public string SelectedChannelName
      {
         get => _SelectedChannelName;
         set
         {
            Set(() => SelectedChannelName, ref _SelectedChannelName, value);
            SelectedChannel = value == null ? null : new ChannelViewModel(Messenger, value);
         }
      }


      private ChannelViewModel _SelectedChannel;
      public ChannelViewModel SelectedChannel
      {
         get => _SelectedChannel;
         set { Set(() => SelectedChannel, ref _SelectedChannel, value); }
      }

      #endregion
   }
}
