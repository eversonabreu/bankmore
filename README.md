# bankmore

dotnet ef migrations add CurrentAccountInitialCreate --output-dir Database/Migrations
dotnet ef migrations add SeedInitialCurrentAccount --output-dir Database/Migrations

na pasta raiz
docker build -t bankmore-current-account-api -f BankMore.CurrentAccount/Dockerfile .
docker run -d -p 8125:8080 --name bankmore-current-account-api -e ASPNETCORE_ENVIRONMENT=Development bankmore-current-account-api
