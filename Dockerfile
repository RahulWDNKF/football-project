# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and publish
COPY . ./
RUN dotnet publish -c Release -o /app/out

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/out .

# Render uses dynamic PORT (important)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Start app
ENTRYPOINT ["dotnet", "FootballDashboardAPI.dll"]
