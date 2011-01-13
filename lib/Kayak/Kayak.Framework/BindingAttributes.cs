﻿using System;
using System.Linq;
using System.Reflection;

namespace Kayak.Framework
{
    /// <summary>
    /// Decorate a method with this attribute to indicate that it should be invoked to handle
    /// requests for a given path.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PathAttribute : Attribute
    {
        public string Path { get; private set; }
        public PathAttribute(string path) { Path = path; }

        public static string[] PathsForMethod(MethodInfo method)
        {
            return (from pathAttr in method.GetCustomAttributes(typeof(PathAttribute), false)
                    select (pathAttr as PathAttribute).Path).ToArray();
        }
    }

    /// <summary>
    /// This attribute is used in conjunction with the [Path] attribute to indicate that the method should be
    /// invoked in response to requests for the path with a particular HTTP verb.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class VerbAttribute : Attribute
    {
        public string Verb { get; private set; }
        public VerbAttribute(string verb) { Verb = verb; }

        public static string[] VerbsForMethod(MethodInfo method)
        {
            return (from verbAttr in method.GetCustomAttributes(typeof(VerbAttribute), false)
                          select (verbAttr as VerbAttribute).Verb).ToArray();
        }
    }

    /// <summary>
    /// Indicates that a method parameter's value is contained within the request body.
    /// </summary>
    public class RequestBodyAttribute : Attribute
    {
        public static bool IsDefinedOn(ParameterInfo pi)
        {
            return pi.GetCustomAttributes(typeof(RequestBodyAttribute), false).Length > 0;
        }

        public static bool IsDefinedOnParameters(MethodInfo mi)
        {
            return mi.GetParameters().Where(pi => RequestBodyAttribute.IsDefinedOn(pi)).Count() > 0;
        }
    }
}
