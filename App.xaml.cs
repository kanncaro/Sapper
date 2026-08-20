using Microsoft.Extensions.DependencyInjection;
using saper1.IServices;
using saper1.Services;
using System.Windows;

namespace saper1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IThemeManager, ThemeManager>();
            services.AddSingleton<IGameTimer, GameTimer>();
            services.AddSingleton<IMinePlacer, MinePlacer>();
            services.AddSingleton<IMineCounter, MineCounter>();
            services.AddSingleton<IGridBuilder, GridBuilder>();
            services.AddSingleton<IGameLogicController, GameLogicController>();

            services.AddTransient<MainWindow>();

            var provider = services.BuildServiceProvider();

            var settings = provider.GetRequiredService<ISettingsService>();
            settings.Load();

            var themeManager = provider.GetRequiredService<IThemeManager>();
            themeManager.ApplyTheme(settings.SettingsData.Theme, Resources);

            var main = provider.GetRequiredService<MainWindow>();
            main.Show();
        }
    }

}
