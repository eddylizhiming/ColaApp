using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaApp.Client.ViewModels.Design
{
    class LoginDesignViewModel : LoginViewModel
    {
        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static LoginDesignViewModel Instance => new LoginDesignViewModel();

        public LoginDesignViewModel() : base(null)
        {
            this.UserName = "Cola";
        }
    }
}
