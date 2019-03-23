using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ColaApp.Client.ViewModels.Base
{
    /// <summary>
    /// 校验ViewModel基类
    /// </summary>
    public class ValidatableViewModelBase : ViewModelBase, IValidatableViewModel
    {
        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        public event EventHandler ValidationTriggered;

        #region Public Methods
        public bool Validate()
        {
            ViewModelBase objectToValidate = this;
            Type objectType = objectToValidate.GetType();
            var oldErrors = new Dictionary<string, ICollection<string>>(_validationErrors);
            _validationErrors.Clear();

            //首先校验之前的错误
            foreach (var error in oldErrors)
            {
                var property = objectType.GetProperty(error.Key);
                if (property.GetCustomAttributes(typeof(ValidationAttribute), true).Any())
                {
                    object value = property.GetValue(objectToValidate, null);
                    if (!(ValidateProperty(property.Name, value) && this.IsStopWhenValidateFail))
                    {
                        return !HasErrors;
                    }
                }
            }
            
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (oldErrors.ContainsKey(property.Name)) //之前已经在OldErrors中校验过了，所以跳过
                    continue;

                if (property.GetCustomAttributes(typeof(ValidationAttribute), true).Any())
                {
                    object value = property.GetValue(objectToValidate, null);
                    if (!(ValidateProperty(property.Name, value) && this.IsStopWhenValidateFail))
                    {
                        break;
                    }
                }
            }

            return !HasErrors;
        }

        /// <summary>
        /// 进行验证，当验证失败时，失败的控件会获得焦点
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Window中需要设置Behavior方才生效
        /// xmlns:baseVM="clr-namespace:ColaApp.Client.ViewModels.Base"
        /// <i:Interaction.Behaviors>
        ///     <baseVM:SetFocusOnValidationErrorBehavior />
        /// </i:Interaction.Behaviors>
        /// </remarks>
        public bool ValidateAndFocus()
        {
            bool hasErrors = !this.Validate();
            if (hasErrors)
                OnValidationTriggered();
            return hasErrors;
        }
        #endregion

        #region INotifyDataErrorInfo Members   
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
            {
                return null;
            }

            return _validationErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return _validationErrors.Count > 0; }
        }

        /// <summary>
        /// 当遇到验证失败时是否终止验证
        /// </summary>
        public bool IsStopWhenValidateFail { get; private set; } = true;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        protected virtual void OnValidationTriggered()
        {
            ValidationTriggered?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected bool ValidateProperty(string propertyName, object value)
        {
            ViewModelBase objectToValidate = this;
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateProperty(
                value,
                new ValidationContext(objectToValidate, null, null)
                {
                    MemberName = propertyName
                },
                results);

            if (isValid)
                RemoveErrorsForProperty(propertyName);
            else
                AddErrorsForProperty(propertyName, results);

            NotifyErrorsChanged(propertyName);
            return isValid;
        }

        private void AddErrorsForProperty(string propertyName, IEnumerable<ValidationResult> validationResults)
        {
            RemoveErrorsForProperty(propertyName);
            _validationErrors.Add(propertyName, validationResults.Select(vr => vr.ErrorMessage).ToList());
        }

        private void RemoveErrorsForProperty(string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);
        }

    }
}
