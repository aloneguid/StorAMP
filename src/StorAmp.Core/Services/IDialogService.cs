using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorAmp.Core.Services
{
   public interface IDialogService
   {
      Task<string> AskStringInputAsync(string title, string message, string initialValue = null);

      Task<bool> AskYesNoAsync(string title, string message);

      IReadOnlyCollection<string> AskLocalFile(string title);

      string AskLocalFolder(string title);

      Task ShowMessageAsync(string title, string message);

      Task ShowPropertiesAsync(string title, Dictionary<string, object> properties);

      /// <summary>
      /// 
      /// </summary>
      /// <param name="title"></param>
      /// <param name="content">Platform specific, supports UserControl </param>
      /// <param name="confirmButtonText"></param>
      /// <param name="confirmCommand"></param>
      /// <returns></returns>
      Task ShowDialogAsync(string title, object content, string confirmButtonText, object confirmCommand);
   }
}
