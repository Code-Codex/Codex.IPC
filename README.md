# Codex.IPC
.NET based IPC using WCF
 This library allows you to use TCP, HTTP, Named Pipe for IPC communication. 
 The samples demonstrate an app where the server is set up to send the current
 CPU and memory usage of the machine to all connected clients.
 
 When the clients start up they register with the server and informs it as to
 which value is it interested in. Once the server see's this registration
 it adds the client to its list and strarts sending it data.
 
 The sample app uses duplex communication, so the server keeps track of all
 the connected clients and has a built in heartbeat to ensure the connection 
 is not servered due to inactivity.
 
 # To use
 1. Build the app in debug mode
 2. Execute the DebugStart.cmd in the sample's directory
