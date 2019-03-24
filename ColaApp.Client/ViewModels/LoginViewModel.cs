using ColaApp.Client.ViewModels.Base;
using Dna;
using Fasetto.Word.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ColaApp.Client.ViewModels
{
    public class LoginViewModel : ViewModelBaseEx
    {
        public ICommand LoginCommand { get; set; }
        #region Public Properties
        [Required(ErrorMessage = "请输入用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }
        /// <summary>
        /// 表明是否在登录中
        /// </summary>
        public bool IsLoginRunning { get; set; }

        #endregion

        private IDialogCoordinator dialogCoordinator;

        public LoginViewModel(IDialogCoordinator coordinator)
        {
            dialogCoordinator = coordinator;
            LoginCommand = new RelayCommand(async() => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            ViewModelLocator.ApplicationViewModel.ServerReachable = false;
            //DI.DI.ApplicationViewModelStatic.ServerReachable = false;

            ValidateAndFocus();

            if (HasErrors)
            {
                return;
            }

            // Call the server and attempt to login with credentials
            var result = await WebRequests.PostAsync<ApiResponse<UserProfileDetailsApiModel>>(
                // Set URL
                RouteHelpers.GetAbsoluteRoute(ApiRoutes.Login),
                // Create api model
                new LoginCredentialsApiModel
                {
                    UserName = this.UserName,
                    Password = this.Password
                });



            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml");
            dialogSettings.SecondAuxiliaryButtonText = "sd";
            dialogSettings.CustomResourceDictionary = dictionary;
        
            await dialogCoordinator.ShowMessageAsync(this, "HEADER", "MESSAGE", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
            //show the dialog
            //var result = await DialogHost.Show("dfdsfdsfds", "RootDialog", ClosingEventHandler);

            ////check the result...
            //Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));


            await RunCommandAsync( ()=> IsLoginRunning, async () =>
            {            
                Console.WriteLine("test login...");
                await Task.Delay(5000);


            });


        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }
    }
}
