# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["PrivacyConfirmedAPI/PrivacyConfirmedAPI.csproj", "PrivacyConfirmedAPI/"]
COPY ["PrivacyConfirmedModel/PrivacyConfirmedModel.csproj", "PrivacyConfirmedModel/"]
COPY ["PrivacyConfirmedBAL/PrivacyConfirmedBAL.csproj", "PrivacyConfirmedBAL/"]
COPY ["PrivacyConfirmedDAL/PrivacyConfirmedDAL.csproj", "PrivacyConfirmedDAL/"]

# Restore dependencies
RUN dotnet restore "PrivacyConfirmedAPI/PrivacyConfirmedAPI.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/PrivacyConfirmedAPI"
RUN dotnet build "PrivacyConfirmedAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PrivacyConfirmedAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy the published application
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "PrivacyConfirmedAPI.dll"]
