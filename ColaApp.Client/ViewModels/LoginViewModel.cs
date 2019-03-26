using ColaApp.Client.ExtensionMethods;
using ColaApp.Client.ViewModels.Base;
using ColaApp.Core;
using Dna;
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

            WebRequestResult<ApiResponse<UserProfileDetailsApiModel>> result = null;
            await RunCommandAsync(() => IsLoginRunning, async () =>
            {
                // Call the server and attempt to login with credentials
                result = await WebRequests.PostAsync<ApiResponse<UserProfileDetailsApiModel>>(
                   // Set URL
                   RouteHelpers.GetAbsoluteRoute(ApiRoutes.Login),
                   // Create api model
                   new LoginCredentialsApiModel
                   {
                       UserName = this.UserName,
                       Password = this.Password
                   });



            });



            // If the response has an error...
            if (await this.HandleErrorIfFailedAsync(result, "登录失败"))
                // We are done
                return;



            // OK successfully logged in... now get users data
            var loginResult = result.ServerResponse.ApiModel;

            // Let the application view model handle what happens
            // with the successful login
            await ViewModelLocator.ApplicationViewModel.HandleSuccessfulLoginAsync(loginResult);

            //MetroDialogSettings dialogSettings = new MetroDialogSettings();
            //var dictionary = new ResourceDictionary();
            //dictionary.Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml");
            //dialogSettings.SecondAuxiliaryButtonText = "sd";
            //dialogSettings.CustomResourceDictionary = dictionary;
        
            //await dialogCoordinator.ShowMessageAsync(this, "HEADER", "MESSAGE", MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
  


        }
    }
}
