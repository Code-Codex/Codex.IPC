echo off
pushd %~dp0
Start "Server" "./Output/IPCTestServer.exe" -b t:p
timeout 5 >nul
Start "Client 1 CPU" "./Output/IPTestClient.exe" -c c
timeout 3 >nul
Start "Client 2 MEM" "./Output/IPTestClient.exe" -c m
timeout 3 >nul
Start "Client 3 ALL" "./Output/IPTestClient.exe" -c a 
popd