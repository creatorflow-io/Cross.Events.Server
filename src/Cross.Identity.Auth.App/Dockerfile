#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 10001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Cross.Identity.Auth.App/Cross.Identity.Auth.App.csproj", "src/Cross.Identity.Auth.App/"]
RUN dotnet restore "./src/Cross.Identity.Auth.App/Cross.Identity.Auth.App.csproj"
COPY . .
WORKDIR "/src/src/Cross.Identity.Auth.App"
RUN dotnet build "./Cross.Identity.Auth.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cross.Identity.Auth.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cross.Identity.Auth.App.dll"]