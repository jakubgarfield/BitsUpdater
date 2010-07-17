using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace BitsUpdatePacker.Data
{
    public sealed class DirectoryExistsValidationRule : ValidationRule
    {
        public String ErrorMessage
        {
            get;
            set;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            bool isValid = false;
            if (value != null)
            {
                isValid = Directory.Exists(value.ToString());
            }
            return new ValidationResult(isValid, (isValid ? null : ErrorMessage));
        }
    }
}
