using ColaApp.Client.ViewModels.Base;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace ColaApp.Client.Views
{
    /// <summary>
    /// 窗体积累
    /// </summary>
    public class BaseWindow : MetroWindow
    {
        public BaseWindow()
        {
            //注册Dialog服务，可以调用对话框，需要绑定ViewModel
            this.SetBinding(DialogParticipation.RegisterProperty, "");

            //添加当验证错误时，控件自动获得焦点的Behavior
            var focusOnValidationErrorBehavior = new SetFocusOnValidationErrorBehavior();
            Interaction.GetBehaviors(this).Add(focusOnValidationErrorBehavior);
        }
    }
}
