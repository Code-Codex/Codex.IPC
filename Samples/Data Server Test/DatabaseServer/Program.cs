using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codex.IPC;
using Codex.IPC.DataTypes;
using Codex.IPC.Server;

namespace DatabaseServer
{
   class Program
   {
      static Thread _IPCServerThread;
      static string username, password, serverName;
      static void Main(string[] args)
      {
         username = ConfigurationManager.AppSettings["username"];
         password = ConfigurationManager.AppSettings["password"];
         serverName = ConfigurationManager.AppSettings["server"];
         ManualResetEvent resetEvent = new ManualResetEvent(false);
         SingleonIPCService.Instance.OnMessageRecieved += Instance_OnMessageRecieved;
         _IPCServerThread = new Thread(ServerThreadLoop);
         _IPCServerThread.Start(resetEvent);
         Console.ReadLine();
         resetEvent.Set();
      }

      static void ServerThreadLoop(object mrevent)
      {
         ManualResetEvent resetEvent = (ManualResetEvent)mrevent;
         var host = new ServerHost();
         host.Start(SingleonIPCService.Instance, resetEvent, new ConnectionOptions("DatabaseServer") { Scheme = BindingScheme.NAMED_PIPE | BindingScheme.TCP });
      }

      private static void Instance_OnMessageRecieved(object sender, SingleonIPCService.MessageRecievedEventArgs e)
      {
         var id = e.Request.Header.ProcessID;
         var startTime = DateTime.Now;

         Trace.WriteLine($"Message: {id} recieved: {startTime}");
         using (SqlConnection connection = new SqlConnection($"Data Source={serverName}; Initial Catalog = AWD; User ID = {username}; Password = {password}; "))
         {
            var query = e.Request.GetBody<string>();
            SqlCommand dbCommand = new SqlCommand(query, connection);
            connection.Open();
            var reader = dbCommand.ExecuteReader();
            var dataset = dataReaderToData(reader);
            e.Response.SetBody<DataSet>(dataset);
         }
         Trace.WriteLine($"Message: {id} finished: {(DateTime.Now - startTime).TotalMilliseconds}");
      }

      private static DataSet dataReaderToData(IDataReader reader)
      {
         DataSet ds = new DataSet();
         while (!reader.IsClosed)
         {
            DataTable dt = new DataTable();
            // DataTable.Load automatically advances the reader to the next result set
            dt.Load(reader);
            ds.Tables.Add(dt);
         }
         return ds;
      }
   }
}
