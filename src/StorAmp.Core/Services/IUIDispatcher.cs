using System;
using System.Collections.Generic;
using System.Text;

namespace StorAmp.Core.Services
{
   public interface IUIDispatcher
   {
      void Invoke(Action action);
   }
}
