FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PINChat.ChatServer/PINChat.ChatServer.csproj", "PINChat.ChatServer/"]
RUN dotnet restore "PINChat.ChatServer/PINChat.ChatServer.csproj"
COPY . .
WORKDIR "/src/PINChat.ChatServer"
RUN dotnet build "PINChat.ChatServer.csproj" -c $Release -o /app/build

FROM build AS publish
RUN dotnet publish "PINChat.ChatServer.csproj" -c $Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PINChat.ChatServer.dll"]