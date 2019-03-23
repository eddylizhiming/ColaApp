using ColaApp.Client.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaApp.Client.ViewModels
{
    public class ApplicationViewModel : ViewModelBaseEx
    {

        /// <summary>
        /// Determines if the application has network access to the server
        /// </summary>
        public bool ServerReachable { get; set; } = true;

        
    }
}
