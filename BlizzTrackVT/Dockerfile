#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BlizzTrackVT/BlizzTrackVT.csproj", "BlizzTrackVT/"]
COPY ["BNetLib/BNetLib.csproj", "BNetLib/"]
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get update && apt-get -y install nodejs
RUN dotnet restore "BlizzTrackVT/BlizzTrackVT.csproj"
COPY . .
WORKDIR "/src/BlizzTrackVT"
RUN npm i && npm i -G webpack webpack-cli
RUN dotnet build "BlizzTrackVT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlizzTrackVT.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlizzTrackVT.dll"]