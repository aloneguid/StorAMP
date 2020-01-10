using System;
using System.Collections.Generic;
using System.Linq;
using Storage.Net.Blobs;

namespace StorAmp.Core.Model.Account
{
   public abstract class ExtraColumnView
   {
      private readonly Dictionary<string, ExtraColumn> _columnNameToColumn = new Dictionary<string, ExtraColumn>();

      protected ExtraColumnView(string name, params ExtraColumn[] columns)
      {
         Name = name;

         foreach(ExtraColumn column in columns)
         {
            _columnNameToColumn[column.Name] = column;
         }
      }

      public string Name { get; }

      public abstract bool IsMatch(Blob anyBlob);

      public IReadOnlyCollection<string> ColumnNames => _columnNameToColumn.Keys.ToList();

      public Func<Blob, string> GetEvaluator(string columnName)
      {
         return _columnNameToColumn[columnName].Value;
      }

      public string GetTooltip(string columnName) => _columnNameToColumn[columnName].Hint;
   }

}
