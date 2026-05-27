# Stage 1: Build cu SDK .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY instagram.csproj ./
RUN dotnet restore instagram.csproj
COPY . .
RUN dotnet publish instagram.csproj -c Release -o /out --no-restore /p:UseAppHost=false

# Stage 2: Runtime - doar ASP.NET, fara SDK (~110MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /out .

# Creeaza folderul pentru imagini uploadate (volum montat)
RUN mkdir -p /app/wwwroot/images

RUN (addgroup -S app 2>/dev/null || true) \
 && (adduser -S -G app app 2>/dev/null || true) \
 && chown -R app:app /app
USER app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "instagram.dll"]
