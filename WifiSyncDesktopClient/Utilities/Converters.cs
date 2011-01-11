using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using LibQdownloader.Utilities;

namespace QDownloader.Utilities
{
    public sealed class Converters
    {
        public static Visibility InvertVisibility(Visibility value)
        {
            return (value == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        public static Visibility ConvertBoolToVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static string ConvertRateToString(long rate)
        {
            return Common.ToReadableSize(rate, 1);
        }

        public static string ConvertSizesToString(long current, long total)
        {
            return ((current <= 0) && (total <= 0)) 
                ? "?"
                : string.Format("{0} of {1}", Common.ToReadableSize(current, 0), Common.ToReadableSize(total, 0));
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Invert)
                return Converters.InvertVisibility(Converters.ConvertBoolToVisibility((bool)value));
            else
                return Converters.ConvertBoolToVisibility((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    public class NullToVisibilityConverter : IValueConverter
    {

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Invert)
                return Converters.InvertVisibility(Converters.ConvertBoolToVisibility(value == null));
            else
                return Converters.ConvertBoolToVisibility(value == null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    public class TransferredSizeofTotalSizeConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Converters.ConvertSizesToString((long)values[0], (long)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    public class TransferredRateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Converters.ConvertRateToString((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ShortenUrlConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Utility.ShortenUrl(((Uri)value).ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ShortenPathConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Utility.ShortenPath((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Converts and enum to boolean based on the Parameter setting
    /// </summary>
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ParameterString = parameter as string;
            if (ParameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object paramvalue = Enum.Parse(value.GetType(), ParameterString);
            if (paramvalue.Equals(value))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ParameterString = parameter as string;
            if (ParameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, ParameterString);
        }
    }

    /// <summary>
    /// If True, returns custom style, otherwise null.
    /// </summary>
    public class BooleanStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            bool valueBool = (bool)value;
            if (valueBool)
                return WifiSyncDesktopClient.App.Current.Resources["ImageSizeBarStyle"];
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
