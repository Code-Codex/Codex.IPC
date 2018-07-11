using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.Interfaces
{
   public interface IConnectionOptions
   {
      /// <summary>
      /// Timeout for opening the connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      TimeSpan OpenTimeout { get; }

      /// <summary>
      /// Timeout for closing the connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      TimeSpan CloseTimeout { get; }

      /// <summary>
      /// Timeout for sending data over the opened connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      TimeSpan SendTimeout { get; }

      /// <summary>
      /// Timeout for receiving data over the opened connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      TimeSpan ReceiveTimeout { get; }

      /// <summary>
      /// Name of the host to connect to.
      /// </summary>
      /// <remarks>
      /// Default is localhost.
      /// </remarks>
      string HostName { get; }

      /// <summary>
      /// Port to use for TCP connections
      /// </summary>
      /// <remarks>
      /// Default is 64000.
      /// </remarks>
      uint TCPPort { get; }

      /// <summary>
      /// Connection schemes that can be used in the server,
      /// this is a flags enum so multiple schemes can be provided.
      /// </summary>
      BindingScheme Scheme { get; }

      /// <summary>
      /// Base address for the process.
      /// </summary>
      string ProcessID { get; }

   }
}
