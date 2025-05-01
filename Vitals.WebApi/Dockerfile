# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Vitals.WebApi.csproj", "./"]
RUN dotnet restore

COPY . .

CMD ls
RUN dotnet build "Vitals.WebApi.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Vitals.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Vitals.WebApi.dll"]

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production