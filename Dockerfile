# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Build the application in Release mode
RUN dotnet publish -c Release -o /out

# Use the official .NET 8 runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output to the runtime container
COPY --from=build /out .

# Expose port 80 for the app
EXPOSE 80

# Define the entrypoint
ENTRYPOINT ["dotnet", "Magnus.Futbot.Api.dll"]
