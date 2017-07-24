using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Codex.IPC.DataTypes
{
   [DataContract]
   public class MessageHeaderBase
   {
      [DataMember]
      private readonly string _messageID;

      /// <summary>
      /// Unique ID for the message.
      /// </summary>
      public string MessageID => _messageID;

      public MessageHeaderBase(string messageID)
      {
         _messageID = messageID;
      }
   }
}
