using System.Windows;
using StorAmp.Core.Services;

namespace StorAmp.Wpf.Services
{
   class SystemService : ISystemService
   {
      public void SetClipboardText(string text) => Clipboard.SetText(text);
   }
}
