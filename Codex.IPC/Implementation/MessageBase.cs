using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            Body = Compress(stream.ToArray());
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
         using (MemoryStream stream = new MemoryStream(Decompress(Body)))
         {
            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
         }
      }


      /// <summary>
      /// Compresses byte array to new byte array.
      /// </summary>
      public static byte[] Compress(byte[] raw)
      {
         using (MemoryStream memory = new MemoryStream())
         {
            using (GZipStream gzip = new GZipStream(memory,
           CompressionMode.Compress, true))
            {
               gzip.Write(raw, 0, raw.Length);
            }
            return memory.ToArray();
         }
      }



      /// <summary>
      /// Decompress a byte array into object
      /// </summary>
      /// <param name="gzip"></param>
      /// <returns></returns>
      public static byte[] Decompress(byte[] gzip)
      {
         using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                          CompressionMode.Decompress))
         {
            const int size = 4096;
            byte[] buffer = new byte[size];
            using (MemoryStream memory = new MemoryStream())
            {
               int count = 0;
               do
               {
                  count = stream.Read(buffer, 0, size);
                  if (count > 0)
                  {
                     memory.Write(buffer, 0, count);
                  }
               }
               while (count > 0);
               return memory.ToArray();
            }
         }
      }
   }
}
