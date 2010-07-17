using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace BitsUpdatePacker.Data
{
    public sealed class StringRequiredValidationRule : ValidationRule
    {
        public String ErrorMessage
        {
            get;
            set;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var isValid = (value ?? String.Empty).ToString().Length > 0;
            return new ValidationResult(isValid, (isValid ? null : ErrorMessage));
        }
    }
}
