

.NET 8 WPF
EF Core 8
Npgsql EF Provider
PostgreSQL 18

## Database

Connection string (used in code):

`Host=localhost;Port=5432;Database=bibl_library;Username=postgres;Password=030609`

Applied migration: `InitialCreate`.

## Run

cd C:\Users\user\Desktop\bibl
dotnet build Bibl.sln
dotnet run --project .\BiblApp\BiblApp.csproj

