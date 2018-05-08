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
      /// Connection schemes that can be used in the server,
      /// this is a flags enum so multiple schemes can be provided.
      /// </summary>
      public BindingScheme Scheme { get; set; }

      /// <summary>
      /// Base address for the process.
      /// </summary>
      public string ProcessID { get; set; }

      /// <summary>
      /// Enable/Disable service discovery
      /// </summary>
      /// <remarks>
      /// Discovery is turned off by default
      /// </remarks>
      public bool EnableDiscovery { get; set; }

      /// <summary>
      /// ctor
      /// </summary>
      public ConnectionOptions(string processID)
      {
         this.ProcessID = processID;
         this.OpenTimeout = new TimeSpan(0, 1, 0);
         this.CloseTimeout = new TimeSpan(0, 1, 0);
         this.SendTimeout = new TimeSpan(0, 1, 0);
         this.ReceiveTimeout = new TimeSpan(0, 1, 0);
         this.HostName = Environment.MachineName;// Constants.LOCAL_HOST;
         this.TCPPort = Constants.TCP_PORT_NUMBER;
         this.HTTPPort = Constants.HTTP_PORT_NUMBER;
         this.Scheme = BindingScheme.NAMED_PIPE;
      }
   }
}
