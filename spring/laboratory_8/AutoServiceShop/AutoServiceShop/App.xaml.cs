using System.Windows;
using AutoServiceShop.DataAccess;

namespace AutoServiceShop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DatabaseInitializer.EnsureDatabaseCreated(); // создание БД при старте
        }
    }
}