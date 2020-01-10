using System;
using System.Collections.Generic;
using System.Text;

namespace StorAmp.Core.Model
{
   public class FileViewerDetails
   {
      public string DisplayName { get; set; }

      public HashSet<string> Extensions { get; set; }
   }
}
