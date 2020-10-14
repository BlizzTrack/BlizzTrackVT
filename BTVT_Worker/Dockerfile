#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BTVT_Worker/BTVT_Worker.csproj", "BTVT_Worker/"]
COPY ["BNetLib/BNetLib.csproj", "BNetLib/"]
COPY ["BTSharedCore/BTSharedCore.csproj", "BTSharedCore/"]
RUN dotnet restore "BTVT_Worker/BTVT_Worker.csproj"
COPY . .
WORKDIR "/src/BTVT_Worker"
RUN dotnet build "BTVT_Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BTVT_Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BTVT_Worker.dll"]