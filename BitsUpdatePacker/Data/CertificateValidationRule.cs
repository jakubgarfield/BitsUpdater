using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Reflection;

namespace BitsUpdatePacker.Data
{
    public sealed class CertificateValidationRule : ValidationRule
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
                isValid = true;
                try
                {
                    using (var certificate = new FileStream(value.ToString(), FileMode.Open, FileAccess.Read))
                    {
                        var publicKey = new StrongNameKeyPair(certificate).PublicKey;
                    }
                }
                catch (IOException)
                {
                    isValid = false;
                }
                catch (ArgumentException)
                {
                    isValid = false;
                }
            }
            return new ValidationResult(isValid, (isValid ? null : ErrorMessage));
        }
    }
}
