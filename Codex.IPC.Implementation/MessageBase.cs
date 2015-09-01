using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Implementation
{
    [DataContract]
    public abstract class MessageBase
    {
        [DataMember]
        public Byte[] Body { get; set; }

        public void SetBody<T>(T data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
                Body = stream.ToArray();
            }
        }

        public T GetBody<T>()
        {
            using (MemoryStream stream = new MemoryStream(Body))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
