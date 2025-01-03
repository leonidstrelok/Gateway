﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthService.API/AuthService.API.csproj", "AuthService.API/"]
COPY ["AuthService.API.DAL/AuthService.API.DAL.csproj", "AuthService.API.DAL/"]
COPY ["AuthService.API.Model/AuthService.API.Model.csproj", "AuthService.API.Model/"]
COPY ["AuthService.API.BLL/AuthService.API.BLL.csproj", "AuthService.API.BLL/"]
RUN dotnet restore "AuthService.API/AuthService.API.csproj"
COPY . .
WORKDIR "/src/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.API.dll"]
