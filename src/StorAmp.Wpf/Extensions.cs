using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;

namespace StorAmp.Wpf
{
   static class Extensions
   {
      public static void ShellOpen(this string s)
      {
         var p = new Process();
         p.StartInfo.UseShellExecute = true;
         p.StartInfo.FileName = s;
         p.Start();
      }

      public static void CopyToClipboard(this string s)
      {
         if(s == null) return;

         var thread = new Thread(() => Clipboard.SetText(s));
         thread.SetApartmentState(ApartmentState.STA);
         thread.Start();
         thread.Join();
      }
   }
}
