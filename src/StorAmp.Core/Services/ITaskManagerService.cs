using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StorAmp.Core.Tasks;

namespace StorAmp.Core.Services
{
   public interface ITaskManagerService
   {
      Task ScheduleAsync(BackgroundTask task);
   }
}
