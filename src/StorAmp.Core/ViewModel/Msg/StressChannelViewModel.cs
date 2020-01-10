using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using Storage.Net.Messaging;

namespace StorAmp.Core.ViewModel.Msg
{
   public class StressChannelViewModel : ViewModelBase
   {
      private readonly IMessenger _messenger;

      public StressChannelViewModel()
      {

      }

      public StressChannelViewModel(IMessenger messenger, string channelName)
      {
         _messenger = messenger;
         ChannelName = channelName;
         MessagesToSend = 10000;
      }

      public string ChannelName { get; }


      private int _MessagesToSend;
      public int MessagesToSend
      {
         get => _MessagesToSend;
         set { Set(() => MessagesToSend, ref _MessagesToSend, value); }
      }
   }
}
