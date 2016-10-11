echo off
pushd %~dp0
Start "Server" "./Output/DatabaseServer.exe"
Start "Client 1" "./Output/DatabaseClient.exe"
Start "Client 2" "./Output/DatabaseClient.exe"
Start "Client 3" "./Output/DatabaseClient.exe"
Start "Client 3" "./Output/DatabaseClient.exe"
Start "Client 3" "./Output/DatabaseClient.exe"
popd