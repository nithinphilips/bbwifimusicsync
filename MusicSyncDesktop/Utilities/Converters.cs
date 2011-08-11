/**********************************************************************
 * WifiMusicSync
 * Copyright (C) 2011 Nithin Philips <nithin@nithinphilips.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 **********************************************************************/

using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using LibQdownloader.Utilities;

namespace WifiSyncDesktop.Utilities
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
                return App.Current.Resources["ImageSizeBarStyle"];
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
