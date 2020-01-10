using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorAmp.Core.Services
{
   public interface IProgressReportingService
   {
      Task StartProgressAsync(string title, string content, double min, double max);

      Task UpdateProgressAsync(string message, double progress);
   }
}
