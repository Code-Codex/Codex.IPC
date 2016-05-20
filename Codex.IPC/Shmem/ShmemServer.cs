using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Shmem
{
    /// <summary>
    /// Manages the shared memory for the process.
    /// </summary>
    public sealed class ShmemServer : IDisposable
    {
        /// <summary>
        /// Postfix used for creating the memory mapped file.
        /// </summary>
        internal const string SHMENM_POSTFIX = "CodexShmem";

        /// <summary>
        /// Local instance of the server
        /// </summary>
        private static ShmemServer _instance;

        /// <summary>
        /// MEmory mapped managed interface.
        /// </summary>
        private MemoryMappedFile _shmem;

        private bool disposed = false; // to detect redundant calls

        private static object _syncLock = new object();

        /// <summary>
        /// Gets the singleton instance of the shmem server.
        /// </summary>
        public static ShmemServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                            _instance = new ShmemServer();
                    }
                }
                return _instance;
            }
        }

        private ShmemServer()
        {

        }

        ~ShmemServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Creates and initializes the Shared memory block.
        /// </summary>
        /// <param name="size">Size of the shared memory in bytes</param>
        public void Initialize(long size)
        {
            var process = Process.GetCurrentProcess();
            string shmemName = GetShmemName(process.Id);
            if (_shmem != null)
            {
                _shmem.Dispose();
                _shmem = null;
            }
            _shmem = MemoryMappedFile.CreateNew(shmemName, size);
        }

        /// <summary>
        /// Gets the name of the shared memory for the process.
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static string GetShmemName(int processID)
        {
            return $"{processID}-{SHMENM_POSTFIX}";
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_shmem != null)
                    {
                        _shmem.Dispose();
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Reads a structure array from the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="count">Array size</param>
        /// <returns>An array of type T objects.</returns>
        public T[] GetData<T>(long offset, int count) where T : struct
        {
            return _shmem.GetData<T>(offset, count);
        }

        /// <summary>
        /// Writes a structure to the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmemName">Name of the shared memory</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="data">Structure to write</param>
        public void SetData<T>(string shmemName, long offset,  T data) where T :struct
        {
            _shmem.SetData<T>(offset, data);
        }

        /// <summary>
        /// Writes a structure array to the shared memory.
        /// </summary>
        /// <typeparam name="T">Generic structure type.</typeparam>
        /// <param name="shmemName">Name of the shared memory</param>
        /// <param name="offset">Offset location where the structure is located.</param>
        /// <param name="data">Structure array to write</param>
        public void SetData<T>(string shmemName, long offset, T[] data) where T : struct
        {
            _shmem.SetData<T>(offset, data);
        }
    }
}
