using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC
{
   [Flags]
   public enum BindingScheme
   {
      [Description("net.tcp")]
      TCP = 0x01,
      [Description("net.pipe")]
      NAMED_PIPE = 0x02
   }
}
