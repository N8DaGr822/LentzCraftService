# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["LentzCraftServices.Domain/LentzCraftServices.Domain.csproj", "LentzCraftServices.Domain/"]
COPY ["LentzCraftServices.Infrastructure/LentzCraftServices.Infrastructure.csproj", "LentzCraftServices.Infrastructure/"]
COPY ["LentzCraftServices/LentzCraftServices.csproj", "LentzCraftServices/"]
RUN dotnet restore "LentzCraftServices/LentzCraftServices.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/LentzCraftServices"
RUN dotnet build "LentzCraftServices.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LentzCraftServices.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Run the app
ENTRYPOINT ["dotnet", "LentzCraftServices.dll"]
