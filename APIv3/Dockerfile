FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["APIv3.csproj", ""]
RUN dotnet restore "./APIv3.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "APIv3.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "APIv3.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "APIv3.dll"]