#Grabs a blank image file from microsoft
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

#Exposes port 80
ENV ASPNETCORE_URLS=http://+:80

#Grabs blank docker image for building
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AuthenticationAPI.csproj", "./"]
RUN dotnet restore "AuthenticationAPI.csproj"
COPY . .
RUN dotnet build "AuthenticationAPI.csproj" -c Release -o /app/build
#Uses Release instead of build
FROM build AS publish
RUN dotnet publish "AuthenticationAPI.csproj" -c Release -o /app/publish

#Creates Image file from the entry point.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationAPI.dll"]
