# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# run as built-in non-root user 'app' to avoid permission issues
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["EVChargingStation.CARC.WebAPI.TruongNN/EVChargingStation.CARC.WebAPI.TruongNN.csproj", "EVChargingStation.CARC.WebAPI.TruongNN/"]
COPY ["EVChargingStation.CARC.Application.TruongNN/EVChargingStation.CARC.Application.TruongNN.csproj", "EVChargingStation.CARC.Application.TruongNN/"]
COPY ["EVChargingStation.CARC.Infrastructure.TruongNN/EVChargingStation.CARC.Infrastructure.TruongNN.csproj", "EVChargingStation.CARC.Infrastructure.TruongNN/"]
COPY ["EVChargingStation.CARC.Domain.TruongNN/EVChargingStation.CARC.Domain.TruongNN.csproj", "EVChargingStation.CARC.Domain.TruongNN/"]

RUN dotnet restore "EVChargingStation.CARC.WebAPI.TruongNN/EVChargingStation.CARC.WebAPI.TruongNN.csproj"

# Copy all and build
COPY . .
WORKDIR "/src/EVChargingStation.CARC.WebAPI.TruongNN"
RUN dotnet build "EVChargingStation.CARC.WebAPI.TruongNN.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EVChargingStation.CARC.WebAPI.TruongNN.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EVChargingStation.CARC.WebAPI.TruongNN.dll"]
