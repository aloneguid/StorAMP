using System;
using Microsoft.Azure.KeyVault.Models;

namespace StorAmp.Wpf.Wpf.Viewers
{
   public interface IFileViewer
   {
      void SetContentFromFile(string filePath, string formatHint);

      void Stop();
   }

   public interface IFileEditor : IFileViewer
   {
      event Action<bool> FileChanged;

      void SaveContentToFile(string filePath);
   }
}
