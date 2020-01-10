using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorAmp.Core.Tasks
{
   class NeverEndingTask : BackgroundTask
   {
      public NeverEndingTask() : base("neverending")
      {
      }

      public override async Task ExecuteAsync()
      {
         while(true)
         {
            Message = "indeterminate";
            IsIndeterminate = true;

            await Task.Delay(TimeSpan.FromSeconds(5));

            Message = "with progress";
            IsIndeterminate = false;
            
            for(int i = 0; i < 100; i++)
            {
               UpdateProgress(i, 100);
               await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
         }
      }
   }
}
