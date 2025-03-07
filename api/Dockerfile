# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the entire project and build it
COPY . .
WORKDIR "/src"
RUN dotnet build "Plutus.API/Plutus.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Plutus.API/Plutus.API.csproj" -c Release -o /app/publish

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set default environment variables (optional)
# Set the ASP.NET Core URLs to listen on all interfaces
ENV ASPNETCORE_URLS="http://+:80"

# Expose the port the app runs on
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Plutus.API.dll"]

