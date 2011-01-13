using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WifiSyncDesktop.QuickZip
{
    public class PathNotEmptyRule : ValidationRule
    {
        public static PathNotEmptyRule Instance = new PathNotEmptyRule();

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                if (!(value is string))
                    return new ValidationResult(false, "InvalidPath");

                if (string.IsNullOrEmpty((string)value))
                    return new ValidationResult(false, "Path is empty");
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, "Invalid Path");
            }

            return new ValidationResult(true, null);
        }
    }
}
