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
    public static class ShmemClient
    {
        /// <summary>
        /// Reads a structure array from the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmemName">Name of the shared memory.</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="count">Array size</param>
        /// <returns>An array of type T objects.</returns>
        public static T[] GetData<T>(string shmemName, long offset, int count) where T : struct
        {
            using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
            {
                return shmem.GetData<T>(offset, count);
            }
        }

        /// <summary>
        /// Reads a structure array from the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmem">The memory mapped file to read from.</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="count">Array size</param>
        /// <returns>An array of type T objects.</returns>
        internal static T[] GetData<T>(this MemoryMappedFile shmem, long offset, int count) where T : struct
        {
            using (var accesor = shmem.CreateViewAccessor())
            {
                T[] array = new T[count];
                accesor.ReadArray(offset, array, 0, count);
                return array;
            }
        }

        /// <summary>
        /// Writes a structure to the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmemName">Name of the shared memory</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="data">Structure to write</param>
        public static void SetData<T>(string shmemName, long offset,T data) where T :struct
        {
            using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
            {
                shmem.SetData<T>(offset,data);
            }
        }

        /// <summary>
        /// Writes a structure array to the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmemName">Name of the shared memory</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="data">Structure array to  write</param>
        public static void SetData<T>(string shmemName, long offset, T[] data) where T : struct
        {
            using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
            {
                shmem.SetData<T>(offset, data);
            }
        }


        /// <summary>
        /// Writes a structure to the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmem">The memory mapped file to read from.</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="data">Structure to write</param>
        internal static void SetData<T>(this MemoryMappedFile shmem, long offset, T data) where T : struct
        {
            using (var accesor = shmem.CreateViewAccessor())
            {
                accesor.Write<T>(offset, ref data);
            }
        }


        /// <summary>
        /// Writes a structure to the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmem">The memory mapped file to read from.</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="data">Structure array to  write</param>
        internal static void SetData<T>(this MemoryMappedFile shmem, long offset, T[] data) where T : struct
        {
            using (var accesor = shmem.CreateViewAccessor())
            {
                int size = System.Runtime.InteropServices.Marshal.SizeOf<T>();
                for (int i = 0; i < data.Count(); i++)
                    accesor.Write<T>(offset + (size* i) , ref data[i]);
            }
        }
    }
}
