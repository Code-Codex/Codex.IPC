using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Shmem
{
   /// <summary>
   /// Client for accessing the Shared memory
   /// </summary>
   public static class Shmem
   {
      /// <summary>
      /// Writes a structure to the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmemName">Name of the shared memory</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <param name="data">Structure to write</param>
      public static void SetData<T>(string shmemName, long position, T data) where T : struct
      {
         using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
         {
            shmem.SetData(position, data);
         }
      }

      /// <summary>
      /// Reads a structure from the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmemName">Name of the shared memory.</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <returns>An item of type T.</returns>
      public static T GetData<T>(string shmemName, long position) where T : struct
      {
         using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
         {
            return shmem.GetData<T>(position);
         }
      }

      /// <summary>
      /// Writes a structure array to the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmemName">Name of the shared memory</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <param name="data">Structure array to  write</param>
      public static void SetData<T>(string shmemName, long position, T[] data) where T : struct
      {
         using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
         {
            shmem.SetData(position, data);
         }
      }

      /// <summary>
      /// Reads a structure array from the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmemName">Name of the shared memory.</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <param name="count">Array size</param>
      /// <returns>An array of type T objects.</returns>
      public static T[] GetData<T>(string shmemName, long position, int count) where T : struct
      {
         using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
         {
            return shmem.GetData<T>(position, count);
         }
      }

      /// <summary>
      /// Writes a structure to the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmem">The memory mapped file to read from.</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <param name="data">Structure to write</param>
      public static void SetData<T>(this MemoryMappedFile shmem, long position, T data) where T : struct
      {
         using (var accesor = shmem.CreateViewAccessor())
         {
            accesor.Write(position, ref data);
         }
      }

      /// <summary>
      /// Reads a structure array from the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmem">The memory mapped file to read from.</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <returns>An item of type T.</returns>
      public static T GetData<T>(this MemoryMappedFile shmem, long position) where T : struct
      {
         using (var accesor = shmem.CreateViewAccessor())
         {
            T item;
            accesor.Read(position, out item);
            return item;
         }
      }


      /// <summary>
      /// Writes a structure to the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmem">The memory mapped file to read from.</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <param name="data">Structure array to  write</param>
      public static void SetData<T>(this MemoryMappedFile shmem,long position, T[] data) where T : struct
      {
         using (var accesor = shmem.CreateViewAccessor())
         {
            accesor.WriteArray(position, data, 0, data.Length);
         }
      }

      /// <summary>
      /// Reads a structure array from the shared memory.
      /// </summary>
      /// <typeparam name="T">Generic structure type.</typeparam>
      /// <param name="shmem">The memory mapped file to read from.</param>
      /// <param name="position">Position where the structure is located.</param>
      /// <param name="count">Array size</param>
      /// <returns>An array of type T objects.</returns>
      public static T[] GetData<T>(this MemoryMappedFile shmem,long position, int count) where T : struct
      {
         using (var accesor = shmem.CreateViewAccessor())
         {
            T[] array = new T[count];
            accesor.ReadArray(position, array, 0, count);
            return array;
         }
      }
   }
}
