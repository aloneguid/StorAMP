using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorAmp.Core.Services.Az
{
   public interface IUserTokenInteraction
   {
      string UserCode { get; set; }
      string VerificationUrl { get; set; }

      Task CompleteAuthAsync();
   }
}
