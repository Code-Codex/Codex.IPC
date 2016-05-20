using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    [Flags]
    public enum BindingScheme
    {
        TCP = 0x01,
        HTTP = 0x02,
        NAMED_PIPE = 0x04
    }
}
