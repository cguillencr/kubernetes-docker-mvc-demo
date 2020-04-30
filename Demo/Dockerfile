FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Demo.csproj", ""]
RUN dotnet restore "./Demo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Demo.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Demo.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Demo.dll"]