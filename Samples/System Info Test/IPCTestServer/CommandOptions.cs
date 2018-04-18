using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCTestServer
{
   /// <summary>
   /// Command line switches accepted by the application
   /// </summary>
   public class CommandOptions
   {
      [Option('n',"name",HelpText ="Name for the server instance",Default =nameof(IPCTestServer))]
      public string ServerName { get; set; }


      [Option('b', "bind", HelpText = "Schemes to use for exposing api t=tcp, h=http, p=pipes", Separator = ':', Required =true)]
      public IEnumerable<char> BindingScheme { get; set; } = new List<char>();

   }
}
