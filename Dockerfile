# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["PrivacyConfirmed/PrivacyConfirmed.csproj", "PrivacyConfirmed/"]
COPY ["PrivacyConfirmedBAL/PrivacyConfirmedBAL.csproj", "PrivacyConfirmedBAL/"]
COPY ["PrivacyConfirmedDAL/PrivacyConfirmedDAL.csproj", "PrivacyConfirmedDAL/"]
COPY ["PrivacyConfirmedModel/PrivacyConfirmedModel.csproj", "PrivacyConfirmedModel/"]

# Restore
RUN dotnet restore "PrivacyConfirmed/PrivacyConfirmed.csproj"

# Copy everything
COPY . .

# Build
WORKDIR /src/PrivacyConfirmed
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# IMPORTANT → match your app port (we will adjust after logs)
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "PrivacyConfirmed.dll"]