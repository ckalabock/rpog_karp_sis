using System.Windows;
using BiblApp.Data;
using Microsoft.EntityFrameworkCore;

namespace BiblApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            using var db = new LibraryContext();
            db.Database.Migrate();
            SeedData.EnsureSeeded(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Database startup error: {ex.Message}",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
            return;
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}
