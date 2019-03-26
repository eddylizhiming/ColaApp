using ColaApp.Client.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColaApp.Core;

namespace ColaApp.Client.ExtensionMethods
{
    /// <summary>
    /// Extension methods for the <see cref="UserProfileDetailsApiModel"/> class
    /// </summary>
    public static class UserProfileDetailsApiModelExtension
    {
        /// <summary>
        /// Creates a new <see cref="LoginCredentialsDataModel"/>
        /// from this model
        /// </summary>
        /// <returns></returns>
        public static LoginCredentialsDataModel ToLoginCredentialsDataModel(this UserProfileDetailsApiModel apiModel)
        {
            return new LoginCredentialsDataModel
            {
                Email = apiModel.Email,
                Username = apiModel.Username,
                Token = apiModel.Token
            };
        }

    }
}
