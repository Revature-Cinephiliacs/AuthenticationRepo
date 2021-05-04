FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AuthenticationAPI/AuthenticationAPI.csproj", "AuthenticationAPI/"]
RUN dotnet restore "AuthenticationAPI/AuthenticationAPI.csproj"
COPY . .
WORKDIR "/src/AuthenticationAPI"
RUN dotnet build "AuthenticationAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationAPI.dll"]
