using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StorAmp.Core.Model;

namespace StorAmp.Core.Services
{
   public interface IThemeManagerService
   {
      IEnumerable<string> GetBaseColorNames();

      IEnumerable<ColorScheme> GetColorSchemes();

      void ApplyColorScheme(string baseColorName, ColorScheme colorScheme);
   }
}
