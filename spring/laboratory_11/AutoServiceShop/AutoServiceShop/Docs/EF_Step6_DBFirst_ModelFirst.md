## Step 6: DB First и Model First (что показать в лабораторной)

### 1) Подготовить БД
В проекте создание БД делается автоматически при старте приложения:
`App.xaml.cs` -> `DatabaseInitializer.EnsureDatabaseCreated();`

Перед тем как делать reverse engineering (DB First), убедитесь, что файл `AutoService.db` реально создан в рабочей папке приложения.

### 2) DB First (сгенерировать сущности и DbContext из БД)

Вариант для EF Core (reverse engineering через `dotnet ef`):

1. Убедитесь, что создана БД `AutoService.db`.
2. Выполните команду из папки проекта:

```powershell
dotnet ef dbcontext scaffold "Data Source=AutoService.db" Microsoft.EntityFrameworkCore.Sqlite `
  --context AutoServiceDbContextScaffolded `
  --output-dir DataAccess\EfCore\Scaffolded `
  --context-dir DataAccess\EfCore\Scaffolded `
  --force
```

После этого в `DataAccess/EfCore/Scaffolded` появятся сгенерированные сущностные классы и отдельный `DbContext`.

### 3) Model First (сгенерировать БД/миграции из модели)

В EF Core “Model First” обычно демонстрируют через миграции:

1. Измените модель в `AutoServiceDbContext` (сущности/связи/поля).
2. Создайте миграцию:

```powershell
dotnet ef migrations add ModelFirstDemo --context AutoServiceDbContext
```

3. Примените миграцию (опционально, если нужно обновить БД):

```powershell
dotnet ef database update --context AutoServiceDbContext
```

Так вы покажете генерацию SQL/изменений схемы “из модели”.

### 4) Про “EDM/EDMX”

Если преподаватель требует именно файл `.edmx`, то это относится к EF6 (Entity Framework 6) + дизайнеру в Visual Studio.
EF Core напрямую `.edmx` не генерирует тем же способом.

В этом проекте используется EF Core, поэтому для “EDM/EDMX” обычно показывают:
- DB First: `dbcontext scaffold` (п.2 выше)
- Model First: миграции (п.3 выше)

