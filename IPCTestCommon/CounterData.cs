using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCTestCommon
{
    [Serializable]
    public struct CounterData
    {
        public CounterType Type { get; set; }
        public float Value { get; set; }
    }
}
