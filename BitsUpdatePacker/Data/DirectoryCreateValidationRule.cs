using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace BitsUpdatePacker.Data
{
    public sealed class DirectoryCreateValidationRule : ValidationRule
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
                var directoryPath = value.ToString();
                if (!Directory.Exists(directoryPath))
                {
                    try
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    catch (IOException)
                    {
                        isValid = false;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                    catch (NotSupportedException)
                    {
                        isValid = false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        isValid = false;
                    }
                }
            }

            return new ValidationResult(isValid, (isValid ? null : ErrorMessage));
        }
    }
}
