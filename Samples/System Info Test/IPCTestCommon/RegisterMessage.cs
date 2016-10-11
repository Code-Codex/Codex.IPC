using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCTestCommon
{
    [Serializable]
    public struct RegisterMessage
    {
        public CounterType Counter { get; set; }
    }
}
