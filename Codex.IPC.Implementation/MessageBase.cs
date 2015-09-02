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
    /// <summary>
    /// Base class fo the messages that are exchanged between the client and the server.
    /// </summary>
    [DataContract]
    public abstract class MessageBase
    {
        /// <summary>
        /// Body of the message, this contains serialized structures.
        /// </summary>
        [DataMember]
        public Byte[] Body { get; set; }

        /// <summary>
        /// Sets the body of the Message.
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="data">Body content</param>
        /// <remarks>
        /// The data should be a value type and should be serializable.
        /// </remarks>
        public void SetBody<T>(T data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
                Body = stream.ToArray();
            }
        }

        /// <summary>
        /// Gets the body of the Message.
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <remarks>
        /// The data should be a value type and should be serializable.
        /// </remarks>
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
