#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 12001
EXPOSE 13001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Cross.TcpServer.App/Cross.TcpServer.App.csproj", "src/Cross.TcpServer.App/"]
RUN dotnet restore "./src/Cross.TcpServer.App/Cross.TcpServer.App.csproj"
COPY . .
WORKDIR "/src/src/Cross.TcpServer.App"
RUN dotnet build "./Cross.TcpServer.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cross.TcpServer.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cross.TcpServer.App.dll"]