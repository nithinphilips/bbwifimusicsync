using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Niche.Utilities
{
    public static class ObjectUtilities
    {

        static public Object ExtractPropertyValue(Object Source, string PropertyPath)
        {
            if (PropertyPath.Contains("."))
            {
                // Name references a sub property
                int index = PropertyPath.IndexOf(".");
                string subObjectName = PropertyPath.Substring(0, index);
                Object subObject = ExtractPropertyValue(Source, subObjectName);
                return ExtractPropertyValue(subObject, PropertyPath.Substring(index + 1));
            }

            // PropertyPath does not contain a "."
            // We are looking for something directly on this object

            Type t = Source.GetType();
            PropertyInfo p = t.GetProperty(PropertyPath);
            if (p != null)
            {
                // We found a Public Property, return it's value
                return p.GetValue(Source, null);
            }

            throw new InvalidOperationException("Did not find value " + PropertyPath + " on " + Source.ToString());
        }

    }
}
