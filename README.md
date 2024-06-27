# TcpeventsServer

## Startup

### Start the Identity Authorization Server

Start the `src/Cross.Identity.App` project with the `https` profile. This project provide the authorization service on port `10001`.

In the first time, you may need to start the IdentityServer project with `/seed` argument to seeding identity data by un-comment the commandLineArgs option in `launchSettings.json`.

```json
// launchsettings.json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      //"commandLineArgs": "/seed",  // Seeding identity data
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:10001"
    },
  },
}
```

### Start the Identity Admin API

Start the `src/Cross.Identity.Admin.App` project with the `https` profile. This project provide the admin API to manage the identity data on port `11001`.
```json
// launchsettings.json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      //"commandLineArgs": "/seed",  // Seeding identity data
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:11001"
    },
  },
}
```
### Start the TCP server

Start the `src/Cross.TcpServer.App` project with the `https` profile. 
This project provides: 
    - The TCP server to handle the raw message from the client on port `13001`.
    - The API to manage the TcpEvents, TcpClients on port `12001`.

### Run the console client

Run the test/Cross.TcpClient.Console project.

### Run the web client

Run the Angular project in the separated repository [tcpevents-client](https://github.com/creatorflow-io/cross-events-client) on port `4200`

## Configuration

### Tcp server configuration

You can change the server configuration in the `appsettings.json` file.

```json
/// appsettings.json
{
  "BackgroundService": {
    "Id": "4a54a3fb-d36d-4672-968a-0ce323b0f9b6", // auto generate if empty
    "FileStore": {
      "Services": [
        {
          "Name": "Tcp Server",
          "AssemblyQualifiedName": "Cross.TcpServer.Core.Network.ServerListener, Cross.TcpServer.Core",
          "ServerOptions": {
            "Port": 13001, // TCP listener port
            "MaxConnections": 100, // Maximum number of connections
            "BufferSize": 1024 // Buffer size to read data
          }
        }
      ]
    }
  }
}
```

### Authority configuration

You can change the authority configuration in the `appsettings.json` file.

```json
/// appsettings.json
{
  "OpenIdConnect": {
	"Authority": "https://localhost:10001"
  }
}
```

### Logging configuration

You can change the logging configuration in the `appsettings.json` file.

```json
/// appsettings.json
{
  "Logging": {
	"LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Cross": "Debug",
      "Startup": "Trace"
    },
    "File": {
      "Directory": "/app/logs", // Log file directory
      "IncludeScopes": false // Include scopes in log
    }
  }
}
```

## Projects structure

### Cross.TcpServer.Core

Handles the TCP connection and send the raw message over the MediatR pipeline.
- [x] Restrict the number of connections.
- [x] Handle the message buffer.
- [x] Metrics for the server.

### Cross.MongoDb

Implement the MongoDB repository pattern.

### Cross.AspNetCore

- [x] Implement WebPush notification service.
- [x] Common authorization policies

### Cross.Events.Abstractions

Define the TcpEvent, TcpClient and the domain events.

### Cross.Events.Api.Contracts

Define the API contracts.
- [x] TcpMessage to format the message to transfer.
- [x] IEventClient define the signalR client.

### Cross.Events.Api

Implement the API.
- [x] Handle the raw message from the TcpServer.
    - [x] Parse the raw message from Tcp server to TcpMessage
    - [x] Register the TcpClient
    - [x] Authorize the client
    - [ ] Limiting the number of messages
    - [x] Send a create TcpEvent command.
- [x] Handle the create TcpEvent command.
- [x] Handle the domain events and send notice to web client.
- [x] Provide the API to manage the TcpEvents, TcpClients.
- [x] SignalR hub to send notice to client.

### Cross.Identity
Define the identity models.

### Cross.Identity.MongoDb
Implement the identity repository pattern and the identity stores for the MongoDB.

### Cross.Identity.Api
Provide the identity API to manage the identity data.

### Cross.TcpClient.Console

A console client to send the TcpMessage to the server.
- [x] Send random messages in a loop.
- [x] Send random messages in parallel.