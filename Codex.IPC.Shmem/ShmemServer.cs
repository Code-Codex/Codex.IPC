using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Shmem
{
    public sealed class ShmemServer : IDisposable
    {
        internal const string SHMENM_POSTFIX = "CodexShmem";

        private static ShmemServer _instance;

        private MemoryMappedFile _shmem;

        private bool disposed = false; // to detect redundant calls

        private static object _syncLock = new object();

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

        public static string GetShmemName(int processID)
        {
            return $"{processID}-{SHMENM_POSTFIX}";
        }

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

        public T[] GetData<T>(long offset, int count) where T : struct
        {
            return _shmem.GetData<T>(offset, count);
        }

        public void SetData<T>(string shmemName, long offset,  T data) where T :struct
        {
            _shmem.SetData<T>(offset, data);
        }

        public void SetData<T>(string shmemName, long offset, T[] data) where T : struct
        {
            _shmem.SetData<T>(offset, data);
        }
    }
}
