using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.DataTypes
{
   /// <summary>
   /// Base class for the messages that are exchanged between the client and the server.
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
            Body = Compress(stream);
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
         using (MemoryStream stream = Decompress(Body))
         {
            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
         }
      }


      /// <summary>
      /// Compresses a memory stream to new byte array.
      /// </summary>
      /// <param name="inputStream">Stream to compress</param>
      /// <returns>Compressed data array</returns>
      public static byte[] Compress(MemoryStream inputStream)
      {
         using (MemoryStream memory = new MemoryStream())
         {
            using (DeflateStream compressedStream = new DeflateStream(memory, CompressionMode.Compress))
               compressedStream.Write(inputStream.ToArray(), 0, (int)inputStream.Length);
            return memory.ToArray();
         }

      }


      /// <summary>
      /// Decompress a byte array into object
      /// </summary>
      /// <param name="input">Data to decompress</param>
      /// <returns>Decompresses memory stream</returns>
      public static MemoryStream Decompress(byte[] input)
      {
         var output = new MemoryStream();
         using (var compressStream = new MemoryStream(input))
         using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
            decompressor.CopyTo(output);

         output.Position = 0;
         return output;
      }
   }
}
