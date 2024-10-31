# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj and any other necessary .csproj files if there are dependencies
COPY ./Magnus.Futbot.Api/Magnus.Futbot.Api.csproj ./Magnus.Futbot.Api/
WORKDIR /app/Magnus.Futbot.Api
RUN dotnet restore

# Return to the root working directory and copy the entire project files
WORKDIR /app
COPY . .

# Change directory to the main project folder and publish
WORKDIR /app/Magnus.Futbot.Api
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
