FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["UI.csproj", ""]
RUN dotnet restore "./UI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "UI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "UI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "UI.dll"]