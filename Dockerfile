# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file from the subfolder
COPY ["FootballDashboardAPI/FootballDashboardAPI.csproj", "FootballDashboardAPI/"]

# Restore dependencies
RUN dotnet restore "FootballDashboardAPI/FootballDashboardAPI.csproj"

# Copy everything
COPY . .

# Publish the app
WORKDIR "/src/FootballDashboardAPI"
RUN dotnet publish "FootballDashboardAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Configure for Render (IMPORTANT)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Start the app
ENTRYPOINT ["dotnet", "FootballDashboardAPI.dll"]
