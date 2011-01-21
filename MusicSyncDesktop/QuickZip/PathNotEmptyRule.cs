using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace QuickZip.Tools
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

                string path = value as string;

                if (string.IsNullOrEmpty(path))
                    return new ValidationResult(false, "Path is empty");

                 bool driveReady = false;
                 DriveInfo di = null;

                 if (!string.IsNullOrWhiteSpace(path) && path.Length > 1)
                 {
                     di = new DriveInfo(path.Substring(0, 1));
                     driveReady = di.IsReady;
                 }

                 if (!driveReady)
                 {
                     return new ValidationResult(false, "The drive is not connected");
                 }
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, "Invalid Path");
            }

            return new ValidationResult(true, null);
        }
    }
}
