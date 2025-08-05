using Microsoft.Extensions.DependencyInjection;
using PDFRename.Services;
using PDFRename.ViewModels;
using System.Windows;

namespace PDFRename
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Configure Dependency Injection Container
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application:\n{ex.Message}", 
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register services
            services.AddSingleton<IOptionsStorageService, OptionsStorageService>();
            services.AddSingleton<PdfMetadataService>();
            services.AddSingleton<FileRenameService>();
            
            // Register ViewModels
            services.AddTransient<MainViewModel>();
            
            // Register Views
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
