using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Configuration;

namespace AutoServiceShop.DataAccess.EfCore
{
    /// <summary>
    /// Фабрика для создания экземпляра AutoServiceDbContext во время разработки (миграции).
    /// Используется EF Core Tools для генерации и применения миграций.
    /// </summary>
    public class AutoServiceDbContextFactory : IDesignTimeDbContextFactory<AutoServiceDbContext>
    {
        /// <summary>
        /// Создаёт экземпляр контекста для дизайн-тайм операций.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        /// <returns>Настроенный экземпляр AutoServiceDbContext.</returns>
        public AutoServiceDbContext CreateDbContext(string[] args)
        {
            // Читаем строку подключения из конфигурационного файла App.config
            var connectionString = ConfigurationManager.ConnectionStrings["AutoServiceDb"]?.ConnectionString;

            // Если строка подключения не найдена, используем значение по умолчанию
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Data Source=AutoService.db";
            }

            // Создаём билдер опций и указываем провайдер SQLite
            var optionsBuilder = new DbContextOptionsBuilder<AutoServiceDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            // Опционально: включаем детальные ошибки и чувствительное логирование из AppSettings
            if (bool.TryParse(ConfigurationManager.AppSettings["EfEnableDetailedErrors"], out bool enableDetailed) && enableDetailed)
                optionsBuilder.EnableDetailedErrors();

            if (bool.TryParse(ConfigurationManager.AppSettings["EfEnableSensitiveDataLogging"], out bool enableSensitive) && enableSensitive)
                optionsBuilder.EnableSensitiveDataLogging();

            // Возвращаем новый контекст с собранными опциями
            return new AutoServiceDbContext(optionsBuilder.Options);
        }
    }
}