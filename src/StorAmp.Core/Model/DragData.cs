using System.Collections.Generic;

namespace StorAmp.Core.Model
{
   public class DragData
   {
      public ConnectedAccount Account { get; set; }

      public Dictionary<string, object> Properties { get; set; }
   }
}
