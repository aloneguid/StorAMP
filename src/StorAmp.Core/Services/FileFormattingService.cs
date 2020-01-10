using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace StorAmp.Core.Services
{
   public class FileFormattingService
   {
      private readonly string _fileContents;
      private string _formattedContents;

      public FileFormattingService(string fileContents)
      {
         _fileContents = fileContents;
      }

      public bool CanReformat { get; private set; }

      public string FormattedContents => _formattedContents;

      public void Analyse()
      {
         CanReformat =
            TryGetJson(_fileContents, out _formattedContents) ||
            TryGetXml(_fileContents, out _formattedContents);
      }

      private bool TryGetJson(string contents, out string json)
      {
         try
         {
            json = JToken.Parse(contents).ToString(Newtonsoft.Json.Formatting.Indented);
            return true;
         }
         catch
         {
            json = null;
            return false;
         }
      }

      private bool TryGetXml(string content, out string xml)
      {
         using(var ms = new MemoryStream())
         {
            using(var writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
               var document = new XmlDocument();

               try
               {
                  // Load the XmlDocument with the XML.
                  document.LoadXml(content);

                  writer.Formatting = Formatting.Indented;

                  // Write the XML into a formatting XmlTextWriter
                  document.WriteContentTo(writer);
                  writer.Flush();
                  ms.Flush();

                  // Have to rewind the MemoryStream in order to read
                  // its contents.
                  ms.Position = 0;

                  // Read MemoryStream contents into a StreamReader.
                  var sReader = new StreamReader(ms);

                  // Extract the text from the StreamReader.
                  xml = sReader.ReadToEnd();
                  return true;
               }
               catch
               {
                  xml = null;
                  return false;
               }
            }
         }
      }
   }
}