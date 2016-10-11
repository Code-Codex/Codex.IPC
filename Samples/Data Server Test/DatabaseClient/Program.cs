using Codex.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codex.IPC.DataTypes;
using System.Data;
using System.Diagnostics;
using System.ServiceModel;

namespace DatabaseClient
{
   class Program
   {
      static Thread _IPCClientThread;
      const string SERVER_PROC_ID = "DatabaseServer";
      static bool loop = true;
      static void Main(string[] args)
      {

         _IPCClientThread = new Thread(ClientThreadLoop);
         _IPCClientThread.Start();
         Console.ReadLine();
         loop = false;
      }

      static void ClientThreadLoop(object mrevent)
      {
         do
         {
            try
            {
               // Construct InstanceContext to handle messages on callback interface
               using (var client = ClientHelper.GetClient(SERVER_PROC_ID, new ConnectionOptions()))
               {
                  client.Open();
                  var msg = new RequestMessage();
                  msg.SetBody<string>(
                     $"SELECT TOP {new Random().Next(100, 2000)} * FROM [AWD].[Person].[Person] WHERE [AWD].[Person].[Person].[LastName] LIKE 'B%' ORDER BY LastName");
                  var startTime = DateTime.Now;
                  var reply = client.Call(msg);
                  Trace.WriteLine($"Time: {(DateTime.Now - startTime).TotalMilliseconds}");
                  var dataset = reply.GetBody<DataSet>();
                  Console.WriteLine(dataset.Tables[0].Rows.Count);
                  if (client.State == CommunicationState.Opened)
                     client.Close();
               }
            }
            catch (CommunicationException)
            {
               Thread.Sleep(500);
            }
            catch (Exception ex)
            {
               Console.WriteLine($"{ex.GetType()}-{ex.Message}");
            }
            Thread.Sleep(500);
         } while (loop);

      }
   }
}
