# -------- BUILD STAGE --------
FROM ://microsoft.com AS build
WORKDIR /src

# Copy the project file from the subfolder
COPY ["FootballDashboardAPI/FootballDashboardAPI.csproj", "FootballDashboardAPI/"]

# Restore dependencies
RUN dotnet restore "FootballDashboardAPI/FootballDashboardAPI.csproj"

# Copy everything and publish
COPY . .
WORKDIR "/src/FootballDashboardAPI"
RUN dotnet publish "FootballDashboardAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- RUNTIME STAGE --------
FROM ://microsoft.com AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Configure for Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FootballDashboardAPI.dll"]
