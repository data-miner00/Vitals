# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["Vitals.Core/*.csproj", "Vitals.Core/"]
COPY ["Vitals.Integrations/*.csproj", "Vitals.Integrations/"]
COPY ["Vitals.WebApi/*.csproj", "Vitals.WebApi/"]
RUN dotnet restore Vitals.WebApi

COPY . .

RUN dotnet build "Vitals.WebApi" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Vitals.WebApi" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Vitals.WebApi.dll"]

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production