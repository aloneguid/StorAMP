using System.Collections.Generic;
using System.IO;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using CloudExplorer.Wpf.Interop;
using MahApps.Metro.Controls;

namespace System.Windows
{
   static class InteropExtensions
   {
      private static readonly Dictionary<string, ImageSource> _cachedMedia = new Dictionary<string, ImageSource>();

      public static void SetPlacement(this Window window, string placementXml)
      {
         WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placementXml);
      }

      public static void SetPlacement(this MetroWindow window, string placementXml)
      {
         WindowPlacement.SetPlacement(new WindowInteropHelper(window).Handle, placementXml);
      }


      public static string GetPlacement(this Window window)
      {
         return WindowPlacement.GetPlacement(new WindowInteropHelper(window).Handle);
      }

      public static string GetPlacement(this MetroWindow window)
      {
         return WindowPlacement.GetPlacement(new WindowInteropHelper(window).Handle);
      }

      public static ImageSource GetImageSource(this string name)
      {
         if(name == null)
            return null;

         if(_cachedMedia.TryGetValue(name, out ImageSource result))
            return result;

         //prefer xaml version if exists
         string xamlName = name + ".xaml";
         try
         {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("Media/" + xamlName, UriKind.Relative));

            using(Stream fs = sri.Stream)
            {
               var imageSource = XamlReader.Load(fs) as DrawingImage;
               _cachedMedia[name] = imageSource;
               return imageSource;
            }
         }
         catch(IOException)
         {
         }

         try
         {
            string pngName = name + ".png";
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,/Media/" + pngName);
            bi.EndInit();

            _cachedMedia[name] = bi;
            return bi;
         }
         catch
         {
            //todo: happens in designer, handle appropriately
         }

         return null;
      }
   }
}
