using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BitsUpdatePacker.Data
{
    public sealed class RegexValidationRule : ValidationRule
    {
        private string _pattern;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);

            if (!string.IsNullOrEmpty(_pattern))
            {
                if (!Regex.IsMatch((value ?? String.Empty).ToString(), _pattern))
                {
                    result = new ValidationResult(false, ErrorMessage);
                }
            }

            return result;
        }

        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }

        public String ErrorMessage
        {
            get;
            set;
        }
    }
}
