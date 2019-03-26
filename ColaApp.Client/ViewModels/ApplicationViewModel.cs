using ColaApp.Client.ExtensionMethods;
using ColaApp.Client.ViewModels.Base;
using ColaApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ColaApp.Client.DI.DI;

namespace ColaApp.Client.ViewModels
{
    public class ApplicationViewModel : ViewModelBaseEx
    {

        /// <summary>
        /// Determines if the application has network access to the server
        /// </summary>
        public bool ServerReachable { get; set; } = true;

        #region Public Helper Methods

        /// <summary>
        /// Handles what happens when we have successfully logged in
        /// </summary>
        /// <param name="loginResult">The results from the successful login</param>
        public async Task HandleSuccessfulLoginAsync(UserProfileDetailsApiModel loginResult)
        {
            // Store this in the client data store
            await ClientDataStore.SaveLoginCredentialsAsync(loginResult.ToLoginCredentialsDataModel());

        }

        #endregion

    }
}
