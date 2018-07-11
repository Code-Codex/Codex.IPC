using System;
using System.Collections.Generic;

namespace Codex.IPC.DataTypes
{
   public class ServerOptions : ConnectionOptions
   {
      /// <summary>
      /// Enable/Disable service discovery
      /// </summary>
      /// <remarks>
      /// Discovery is turned off by default
      /// </remarks>
      public bool EnableDiscovery { get; set; }

      /// <summary>
      /// Scopes that should be published with discovery
      /// </summary>
      /// <remarks>
      /// Only used when enable discovery is set tot true
      /// </remarks>
      public readonly Dictionary<string, string> Scopes;

      public ServerOptions(string processID, Dictionary<string, string> scopes)
         : base(processID)
      {
         Scopes = scopes == null ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(scopes, StringComparer.OrdinalIgnoreCase);
      }
   }
}
