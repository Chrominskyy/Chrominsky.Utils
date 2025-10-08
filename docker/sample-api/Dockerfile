# Sample Dockerfile for .NET 8.0 API using Chrominsky.Utils
# This is a multi-stage build for optimal image size

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["YourApi/YourApi.csproj", "YourApi/"]
RUN dotnet restore "YourApi/YourApi.csproj"

# Copy the rest of the files and build
COPY . .
WORKDIR "/src/YourApi"
RUN dotnet build "YourApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "YourApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "YourApi.dll"]
