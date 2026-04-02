# -------- BUILD STAGE --------
FROM ://microsoft.com AS build
WORKDIR /src

# 1. Copy the project file from the subfolder to the build container
COPY ["FootballDashboardAPI/FootballDashboardAPI.csproj", "FootballDashboardAPI/"]

# 2. Restore dependencies
RUN dotnet restore "FootballDashboardAPI/FootballDashboardAPI.csproj"

# 3. Copy the entire repository and move into the project folder
COPY . .
WORKDIR "/src/FootballDashboardAPI"

# 4. Build and publish the release
RUN dotnet publish "FootballDashboardAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- RUNTIME STAGE --------
FROM ://microsoft.com AS final
WORKDIR /app

# 5. Copy the published output from the build stage
COPY --from=build /app/publish .

# 6. Configure for Render (ASP.NET 8.0 uses 8080 by default)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 7. Start the application
ENTRYPOINT ["dotnet", "FootballDashboardAPI.dll"]
