#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["APIv2/APIv2.csproj", "APIv2/"]
RUN dotnet restore "APIv2/APIv2.csproj"
COPY . .
WORKDIR "/src/APIv2"
RUN dotnet build "APIv2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIv2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIv2.dll"]