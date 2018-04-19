using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTestClient
{
   public class CommandOptions
   {
      [Option('c',"counter", HelpText ="Type of counters c=CPU, m=Memory, a=ALL")]
      public char CounterType { get; set; }
   }
}
