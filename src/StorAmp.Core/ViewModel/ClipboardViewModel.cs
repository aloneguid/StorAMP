using GalaSoft.MvvmLight;
using Humanizer;
using StorAmp.Core.Model.Clipboard;

namespace StorAmp.Core.ViewModel
{
   public class ClipboardViewModel : ViewModelBase
   {
      private ClipboardViewModel()
      {

      }

      public static ClipboardViewModel Instance { get; } = new ClipboardViewModel();

      public void Push(BlobsClipboardData clipboardData)
      {
         Data = clipboardData;
         HasData = Data != null;
         if(Data != null)
         {
            ShortStatus = $"{"item".ToQuantity(clipboardData.Blobs.Count)} in clipboard";
         }
      }

      #region [ Properties ]

      private BlobsClipboardData _data;
      public BlobsClipboardData Data
      {
         get => _data;
         set { Set(() => Data, ref _data, value); }
      }


      private bool _hasData;
      public bool HasData
      {
         get => _hasData;
         set { Set(() => HasData, ref _hasData, value); }
      }


      private string _shortStatus;
      public string ShortStatus
      {
         get => _shortStatus;
         set { Set(() => ShortStatus, ref _shortStatus, value); }
      }

      #endregion
   }
}
