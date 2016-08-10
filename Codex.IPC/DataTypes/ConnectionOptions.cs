using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.DataTypes
{
   /// <summary>
   /// Options which are used to establish 
   /// connections between the client and the server.
   /// </summary>
   public class ConnectionOptions
   {
      /// <summary>
      /// Timeout for opening the connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      public TimeSpan OpenTimeout { get; set; }

      /// <summary>
      /// Timeout for closing the connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      public TimeSpan CloseTimeout { get; set; }

      /// <summary>
      /// Timeout for sending data over the opened connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      public TimeSpan SendTimeout { get; set; }

      /// <summary>
      /// Timeout for receiving data over the opened connection.
      /// </summary>
      /// <remarks>
      /// Default is 1 minute.
      /// </remarks>
      public TimeSpan ReceiveTimeout { get; set; }

      /// <summary>
      /// Name of the host to connect to.
      /// </summary>
      /// <remarks>
      /// Default is localhost.
      /// </remarks>
      public string HostName { get; set; }

      /// <summary>
      /// Port to use for TCP connections
      /// </summary>
      /// <remarks>
      /// Default is 64000.
      /// </remarks>
      public uint TCPPort { get; set; }

      /// <summary>
      /// Port to use for HTTP connections
      /// </summary>
      /// <remarks>
      /// Default is 64001.
      /// </remarks>
      public uint HTTPPort { get; set; }

      /// <summary>
      /// ctor
      /// </summary>
      public ConnectionOptions()
      {
         this.OpenTimeout = new TimeSpan(0, 1, 0);
         this.CloseTimeout = new TimeSpan(0, 1, 0);
         this.SendTimeout = new TimeSpan(0, 1, 0);
         this.ReceiveTimeout = new TimeSpan(0, 1, 0);
         this.HostName = Constants.LOCAL_HOST;
         this.TCPPort = Constants.TCP_PORT_NUMBER;
         this.HTTPPort = Constants.HTTP_PORT_NUMBER;
      }
   }
}
