using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaApp.Client.ViewModels.Base
{
    public interface IValidatableViewModel : INotifyDataErrorInfo
    {
        event EventHandler ValidationTriggered;
    }
}
