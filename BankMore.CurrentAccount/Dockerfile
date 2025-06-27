FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankMore.CurrentAccount.API/BankMore.CurrentAccount.API.csproj", "BankMore.CurrentAccount.API/"]
RUN dotnet restore "./BankMore.CurrentAccount.API/BankMore.CurrentAccount.API.csproj"
COPY . .
WORKDIR "/src/BankMore.CurrentAccount.API"
RUN dotnet build "./BankMore.CurrentAccount.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankMore.CurrentAccount.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir SQliteDatabases

ENTRYPOINT ["dotnet", "BankMore.CurrentAccount.API.dll"]