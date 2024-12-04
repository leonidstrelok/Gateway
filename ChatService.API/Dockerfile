FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChatService.API/ChatService.API.csproj", "ChatService.API/"]
COPY ["ChatService.API.DAL/ChatService.API.DAL.csproj", "ChatService.API.DAL/"]
COPY ["ChatService.API.Model/ChatService.API.Model.csproj", "ChatService.API.Model/"]
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
COPY ["ChatService.API.BLL/ChatService.API.BLL.csproj", "ChatService.API.BLL/"]
RUN dotnet restore "ChatService.API/ChatService.API.csproj"
COPY . .
WORKDIR "/src/ChatService.API"
RUN dotnet build "ChatService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChatService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.API.dll"]
