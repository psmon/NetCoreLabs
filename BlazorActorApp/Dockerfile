#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlazorActorApp/BlazorActorApp.csproj", "BlazorActorApp/"]
COPY ["ActorLib/ActorLib.csproj", "ActorLib/"]

RUN dotnet restore "BlazorActorApp/BlazorActorApp.csproj"
COPY . .
WORKDIR "/src/BlazorActorApp"
RUN dotnet build "BlazorActorApp.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "BlazorActorApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorActorApp.dll"]