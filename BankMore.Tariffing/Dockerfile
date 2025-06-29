# SDK image para compilar e rodar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia todos os arquivos da solution (incluindo os compartilhados)
COPY . .

# Publica o projeto API direto na pasta final
RUN dotnet publish "BankMore.Tariffing/BankMore.Tariffing.API/BankMore.Tariffing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Cria pasta para SQLite
RUN mkdir SQliteDatabases

ENTRYPOINT ["dotnet", "BankMore.Tariffing.API.dll"]