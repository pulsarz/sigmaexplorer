#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SigmaExplorer/Server/SigmaExplorer.Server.csproj", "SigmaExplorer/Server/"]
COPY ["SigmaExplorer/Shared/SigmaExplorer.Shared.csproj", "SigmaExplorer/Shared/"]
COPY ["SigmaExplorer/Client/SigmaExplorer.Client.csproj", "SigmaExplorer/Client/"]
RUN dotnet restore "SigmaExplorer/Server/SigmaExplorer.Server.csproj"
COPY . .
WORKDIR "/src/SigmaExplorer/Server"
RUN dotnet build "SigmaExplorer.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SigmaExplorer.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SigmaExplorer.Server.dll"]