// From https://github.com/danielmoore/InpcTemplate/blob/28489e9ed527d2682afe0c1fb157484c36d28950/InpcTemplate/NotifyProperyChangedExtensions.cs
// (C) Daniel Moore <https://github.com/danielmoore>
// No Licence Data Found.

using System;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;

namespace WifiSyncDesktop.Helpers
{
    /// <summary>
    /// Provides extension methods for subscribing to <see cref="INotifyPropertyChanged"/> and
    /// <see cref="INotifyPropertyChanging"/> in a strongly typed manner.
    /// </summary>
    public static class NotifyPropertyChangedExtensions
    {

        private static string GetPropertyName<TSource, TProp>(Expression<Func<TSource, TProp>> propertySelector)
        {
            var memberExpr = propertySelector.Body as MemberExpression;

            if (memberExpr == null) throw new ArgumentException("must be a member accessor", "propertySelector");

            var propertyInfo = memberExpr.Member as PropertyInfo;

            if (propertyInfo == null || propertyInfo.DeclaringType != typeof(TSource))
                throw new ArgumentException("must yield a single property on the given object", "propertySelector");

            return propertyInfo.Name;
        }
    }
}
