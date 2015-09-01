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
    public static class ShmemClient
    {
        public static T[] GetData<T>(string shmemName, long offset, int count) where T : struct
        {
            using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
            {
                return shmem.GetData<T>(offset, count);
            }
        }

        public static void SetData<T>(string shmemName, long offset,T data) where T :struct
        {
            using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
            {
                shmem.SetData<T>(offset,data);
            }
        }

        public static void SetData<T>(string shmemName, long offset, T[] data) where T : struct
        {
            using (var shmem = MemoryMappedFile.OpenExisting(shmemName))
            {
                shmem.SetData<T>(offset, data);
            }
        }


        internal static T[] GetData<T>(this MemoryMappedFile shmem, long offset, int count) where T : struct
        {
            using (var accesor = shmem.CreateViewAccessor())
            {
                //BinaryFormatter formatter = new BinaryFormatter();
                //return (T)formatter.Deserialize(stream);
                T[] array = new T[count];
                accesor.ReadArray(offset, array, 0, count);
                return array;
            }
        }

        internal static void SetData<T>(this MemoryMappedFile shmem, long offset, T data) where T : struct
        {
            using (var accesor = shmem.CreateViewAccessor())
            {
                accesor.Write<T>(offset, ref data);
            }
        }


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
