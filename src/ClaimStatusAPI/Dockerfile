# Stage: runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Stage: build (support cross-platform builds)
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src

# Copy csproj and restore (cache friendly)
COPY ["ClaimStatusAPI.csproj", "./"]
RUN dotnet restore "ClaimStatusAPI.csproj"

# Copy everything and build
COPY . .
RUN dotnet build "ClaimStatusAPI.csproj" -c $configuration -o /app/build

# Stage: publish
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ClaimStatusAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app

# Copy published output
COPY --from=publish /app/publish .

# Run as non-root user
USER app

ENTRYPOINT ["dotnet", "ClaimStatusAPI.dll"]