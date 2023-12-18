using Microsoft.Extensions.DependencyInjection;
using StoreManager.DB_classes;
using StoreManager.Models.Abstract.Classes;
using StoreManager.ViewModels.CLient;
using StoreManager.ViewModels.Core;
using StoreManager.ViewModels.Services;
using StoreManager.ViewModels.Sign;
using StoreManager.ViewModels.StoreInteraction;
using StoreManager.Views.Admin.Interactions.Emulating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.ViewModels.Admin.Interactions.Emulating
{
    public class EmulateUserViewModel : ViewModelBase
    {
        public User user;
        private readonly ServiceProvider _secviceProvider;
        public EmulateUserView EmulateUserView;
        public EmulateUserViewModel(User user)
        {
            this.user = user;

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });
            services.AddSingleton<MainWindowViewModel>(provider => new MainWindowViewModel(user, provider.GetRequiredService<INavigationService>()));


            services.AddSingleton<MainStoreInterationViewModel>();
            services.AddSingleton<ProfileViewModel>();
            services.AddSingleton<ChangeProfileDataViewModel>();

            //Sign
            services.AddSingleton<MainSignViewModel>();
            services.AddSingleton<SignInViewModel>();
            services.AddSingleton<SignUpViewModel>();

            //Client
            services.AddSingleton<CartViewModel>();

            //Admin
            services.AddSingleton<AdminViewModel>();
            services.AddSingleton<ChooseEmulateUserViewModel>();


            //Interaction
            /*services.AddSingleton<CreateCategoryViewModel>();
            services.AddSingleton<CreateDescriptionViewModel>();*/


            bool isEmulated = true;
            services.AddSingleton<INavigationService>(provider => new NavigationService(
                provider.GetRequiredService<Func<Type, ViewModelBase>>(),
                isEmulated
            ));

            services.AddSingleton<Func<Type, ViewModelBase>>(ServiceProvider => viewModelType => (ViewModelBase)ServiceProvider.GetRequiredService(viewModelType));

            _secviceProvider = services.BuildServiceProvider();

            var mainWindow = _secviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
