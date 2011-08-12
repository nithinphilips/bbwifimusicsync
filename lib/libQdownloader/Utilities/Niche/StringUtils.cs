using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Niche.Utilities
{
    static public class StringUtils
    {

        static public string Format(string Template, Object Source)
        {
            // This regular expression should find named parameters of the form {id} or {id:parameter} 

            Regex r = new Regex(@"\{(?<id>\w+(\.\w+)*)(?<param>:[^}]+)?\}");

            MatchCollection mc = r.Matches(Template);
            StringBuilder result = new StringBuilder();

            // Loop over each match, replacing the placeholders with the result

            int index = 0;
            foreach (Match m in r.Matches(Template))
            {
                // Append content from before this match

                if (index < m.Index)
                    result.Append(Template.Substring(index, m.Index - index));

                // Get the details of this match
                Group id = m.Groups["id"];
                Group param = m.Groups["param"];

                // Create a template to pass to String.Format()
                string t = "{0" + param.ToString() + "}";

                // Get the property identified by id
                Object value = ObjectUtilities.ExtractPropertyValue(Source, id.ToString());

                // Format the value and add it to our result
                result.AppendFormat(t, value);

                index = m.Index + m.Length;
            }

            // Copy across any content from after the last match
            if (index < Template.Length)
            {
                result.Append(Template.Substring(index));
            }

            return result.ToString();

        }

    }
}
