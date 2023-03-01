FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CountryService.API/CountryService.API.csproj", "CountryService.API/"]
RUN dotnet restore "CountryService.API/CountryService.API.csproj"
COPY . .
WORKDIR "/src/CountryService.API"
RUN dotnet build "CountryService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CountryService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CountryService.API.dll"]