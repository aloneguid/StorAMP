using System.ComponentModel;
using System.Threading.Tasks;

namespace StorAmp.Core.Model
{
   public interface IAccountPaneViewModel : INotifyPropertyChanged
   {
      ConnectedAccount Account { get; }

      void RestoreSettings(string settings);

      string GetSettings();

      bool AcceptsDrop(DragData data);

      void DropDataAsync(DragData data);
   }
}
