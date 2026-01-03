# Use the official .NET 10 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Set the working directory
WORKDIR /src

# Copy the solution file and project files
COPY ["InternalPlatformApi.sln", "./"]
COPY ["src/InternalPlatform.Api/InternalPlatform.Api.csproj", "src/InternalPlatform.Api/"]
COPY ["src/InternalPlatform.Application/InternalPlatform.Application.csproj", "src/InternalPlatform.Application/"]
COPY ["src/InternalPlatform.Domain/InternalPlatform.Domain.csproj", "src/InternalPlatform.Domain/"]
COPY ["src/InternalPlatform.Infrastructure/InternalPlatform.Infrastructure.csproj", "src/InternalPlatform.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/InternalPlatform.Api/InternalPlatform.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/InternalPlatform.Api"
RUN dotnet build "InternalPlatform.Api.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "InternalPlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 10 ASP.NET Core runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port the application runs on
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "InternalPlatform.Api.dll"]