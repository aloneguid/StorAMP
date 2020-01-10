using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NetBox.Extensions;
using Storage.Net.Messaging;

namespace StorAmp.Core.ViewModel.Msg
{
   public class ChannelViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;
      public ObservableCollection<QueueMessage> PeekedMessages { get; set; } = new ObservableCollection<QueueMessage>();

      public ChannelViewModel()
      {

      }

      public ChannelViewModel(IMessenger messenger, string channelName)
      {
         _messenger = messenger;
         ChannelName = channelName;
         ChannelNameDisplay = channelName.ToUpper();

         RefreshAsync().Forget();
         TimerRefreshAsync().Forget();
      }

      private RelayCommand _RefreshCommand;
      public RelayCommand RefreshCommand => _RefreshCommand ??= new RelayCommand(() => RefreshAsync().Forget());

      public async Task RefreshAsync()
      {
         MessageCount = await _messenger.GetMessageCountAsync(ChannelName);
         IReadOnlyCollection<QueueMessage> messages = null;
         try
         {
            messages = await _messenger.PeekAsync(ChannelName, 32);
         }
         catch(Exception ex)
         {

         }

         PeekedMessages.Clear();
         foreach(QueueMessage msg in messages)
         {
            PeekedMessages.Add(msg);
         }
      }

      private async Task TimerRefreshAsync()
      {
         try
         {
            MessageCount = await _messenger.GetMessageCountAsync(ChannelName);
         }
         catch { }

         await Task.Delay(TimeSpan.FromSeconds(5));

         TimerRefreshAsync().Forget();
      }

      private string _ChannelName;
      public string ChannelName
      {
         get => _ChannelName;
         set { Set(() => ChannelName, ref _ChannelName, value); }
      }


      private string _ChannelNameDisplay;
      public string ChannelNameDisplay
      {
         get => _ChannelNameDisplay;
         set { Set(() => ChannelNameDisplay, ref _ChannelNameDisplay, value); }
      }


      private long _MessageCount;
      public long MessageCount
      {
         get => _MessageCount;
         set { Set(() => MessageCount, ref _MessageCount, value); }
      }
   }
}
