using Codex.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCTestCommon
{
   public class CommandParser
   {
      public IReadOnlyDictionary<string, string> Arguments => _argumentCollection;

      private Dictionary<string, string> _argumentCollection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      public CommandParser(string[] args)
      {
         for (int i = 0; i < args.Length; i = i + 2)
         {
            var trimmedOption = args[i]?.Trim();
            var trimmedValue = args[i + 1]?.Trim();
            var option = getOption(trimmedOption);
            if (option != null)
               _argumentCollection.Add(option, trimmedValue);
         }
      }

      private bool isOption(string value)
      {
         return value.Substring(0, 2) == "--";
      }

      private string getOption(string value)
      {
         if (isOption(value))
            return value.Substring(2);

         return null;
      }
   }
}
