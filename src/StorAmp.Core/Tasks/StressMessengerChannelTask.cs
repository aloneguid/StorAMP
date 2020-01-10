using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using NetBox.Extensions;
using Storage.Net.Messaging;

namespace StorAmp.Core.Tasks
{
   public class StressMessengerChannelTask : BackgroundTask
   {
      private readonly IMessenger _messenger;
      private readonly string _channelName;
      private readonly int _noOfMesssages;
      private int _batchesSent;
      private int _batchesTotal;
      private const int BatchSize = 100;

      public StressMessengerChannelTask(IMessenger messenger, string channelName, int noOfMesssages) : base("channel stress")
      {
         _messenger = messenger;
         _channelName = channelName;
         _noOfMesssages = noOfMesssages;
         Abstract = channelName;
      }

      public override async Task ExecuteAsync()
      {
         List<IEnumerable<int>> batches = Enumerable.Range(0, _noOfMesssages).Chunk(BatchSize).ToList();
         _batchesTotal = batches.Count;

         await Task.WhenAll(batches.Select(SubmitChunkAsync));

         Message = $"{"message".ToQuantity(_noOfMesssages)} sent";
      }

      private async Task SubmitChunkAsync(IEnumerable<int> indexes)
      {
         await _messenger.SendAsync(_channelName,
            indexes.Select(i => QueueMessage.FromText($"text message {i}")));

         UpdateProgress(++_batchesSent, _batchesTotal);
         Message = $"{_batchesSent * BatchSize}/{_noOfMesssages}";
      }
   }
}
