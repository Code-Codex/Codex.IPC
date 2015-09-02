using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCTestCommon
{
    [Serializable]
    [Flags]
    public enum CounterType
    {
        CPU,
        MEMORY
    }
}
