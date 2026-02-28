# BiblApp - Управление библиотекой книг

Приложение для практической работы: WPF + Entity Framework Core + PostgreSQL.

## Что реализовано

- Сущности: `Book`, `Author`, `Genre`
- Связи:
  - один автор -> много книг
  - один жанр -> много книг
- Конфигурация через Fluent API:
  - первичные ключи
  - обязательные поля
  - ограничения длины строк
  - каскадное удаление
- Интерфейс WPF:
  - таблица книг (`DataGrid`)
  - поиск книг по названию
  - фильтрация по автору и жанру
  - отдельные окна CRUD для книг, авторов и жанров

## Требования

- Windows
- .NET SDK 8.0+
- PostgreSQL 18

## Параметры БД (текущие в проекте)

Строка подключения задана в `BiblApp/Data/LibraryContext.cs`:

```text
Host=localhost;Port=5432;Database=bibl_library;Username=postgres;Password=030609
```

Если у вас другой пароль/пользователь/порт, измените строку подключения в этом файле.

## Первый запуск

1. Перейти в папку проекта:

```powershell
cd C:\Users\user\Desktop\bibl
```

2. (Если базы нет) создать базу `bibl_library`:

```powershell
$env:PGPASSWORD='030609'
& 'C:\Program Files\PostgreSQL\18\bin\psql.exe' -U postgres -h localhost -p 5432 -d postgres -c "CREATE DATABASE bibl_library;"
```

3. Собрать проект:

```powershell
dotnet build Bibl.sln
```

4. Запустить приложение:

```powershell
dotnet run --project .\BiblApp\BiblApp.csproj
```

При старте приложение автоматически:
- применяет миграции (`Database.Migrate()`),
- заполняет БД тестовыми данными (15 реальных книг), если таблицы пустые.

## Миграции EF Core

Проект использует локальный инструмент `dotnet-ef` (см. `.config/dotnet-tools.json`).

Примеры команд:

```powershell
cd C:\Users\user\Desktop\bibl

# Установить локальные tools (если нужно)
dotnet tool restore

# Создать миграцию
dotnet tool run dotnet-ef migrations add <MigrationName> --project .\BiblApp\BiblApp.csproj --startup-project .\BiblApp\BiblApp.csproj --output-dir Migrations

# Применить миграции к базе
dotnet tool run dotnet-ef database update --project .\BiblApp\BiblApp.csproj --startup-project .\BiblApp\BiblApp.csproj
```

## Структура проекта

- `BiblApp/Models` - модели сущностей
- `BiblApp/Data` - `DbContext`, фабрика контекста, сидинг
- `BiblApp/Migrations` - миграции EF Core
- `BiblApp/Windows` - окна CRUD
- `BiblApp/MainWindow.xaml` - главное окно приложения

## GitHub

Удаленный репозиторий:

```text
https://github.com/ckalabock/rpog_karp_sis.git
```
