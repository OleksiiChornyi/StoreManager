using Microsoft.Extensions.DependencyInjection;
using StoreManager.ViewModels;
using StoreManager.ViewModels.Admin;
using StoreManager.ViewModels.Admin.Interactions;
using StoreManager.ViewModels.Client;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using StoreManager.ViewModels.Sign;
using StoreManager.ViewModels.StoreInteraction;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _secviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });
            services.AddSingleton<MainWindowViewModel>();
            
            services.AddSingleton<MainStoreInterationViewModel>();

            //Sign
            services.AddSingleton<MainSignViewModel>();
            services.AddSingleton<SignInViewModel>();
            services.AddSingleton<SignUpViewModel>();

            //Client
            services.AddSingleton<ClientViewModel>();

            //Admin
            services.AddSingleton<AdminViewModel>();


            //Interaction
            /*services.AddSingleton<CreateCategoryViewModel>();
            services.AddSingleton<CreateDescriptionViewModel>();*/


            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<Func<Type, ViewModelBase>>(ServiceProvider => viewModelType => (ViewModelBase)ServiceProvider.GetRequiredService(viewModelType));

            _secviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _secviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (Directory.Exists("tmp"))
                    Directory.Delete("tmp", true);
            }
            catch
            {
                MessageBox.Show("I can't delete temporary files from computer");
            }
        }
    }
}
