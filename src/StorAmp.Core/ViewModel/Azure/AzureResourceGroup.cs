using System;
using System.Collections.Generic;
using System.Text;

namespace StorAmp.Core.ViewModel.Azure
{
   class AzureResourceGroup : HierarchicalResource
   {
      public AzureResourceGroup(string name) : base(name, "azure/rg")
      {

      }
   }
}
