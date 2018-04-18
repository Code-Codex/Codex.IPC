echo off
pushd %~dp0
Start "Server" "./Output/IPCTestServer.exe -b t:p"
timeout 5 >nul
Start "Client 1 CPU" "./Output/IPTestClient.exe"
timeout 3 >nul
Start "Client 2 CPU" "./Output/IPTestClient.exe"
timeout 3 >nul
Start "Client 3 MEM" "./Output/IPTestClient.exe" 2
popd