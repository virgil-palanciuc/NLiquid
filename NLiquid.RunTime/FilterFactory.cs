/********************************************************************************
Copyright (c) 2012 Adobe Systems Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NLiquid.Runtime
{
    using LiquidFilter = Func<object, IEnumerable, object>;

    public class FilterFactory
    {
        private static readonly ParameterComparer _comparer = new ParameterComparer();
        private class ParameterComparer : IEqualityComparer<ParameterInfo>
        {
            public bool Equals(ParameterInfo x, ParameterInfo y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (x == null || y == null)
                {
                    return false;
                }

                return x.ParameterType == y.ParameterType;
            }

            public int GetHashCode(ParameterInfo obj)
            {
                return obj.GetHashCode();
            }
        }
        private static readonly Hashtable AllFilters;
        static FilterFactory()
        {
            var allClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t =>  String.Equals(t.Namespace, "NLiquid.Runtime.FilterDefinitions", StringComparison.Ordinal));
            var liquidFilterMethod = typeof (LiquidFilter).GetMethod("Invoke");
            var allFilters =
                allClasses.SelectMany(
                    cls => cls.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                               .Where(m =>
                                      m.ReturnType == liquidFilterMethod.ReturnType &&
                                      m.GetParameters().SequenceEqual(liquidFilterMethod.GetParameters(),_comparer)
                                      ));
            AllFilters = System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable();
            foreach (var filter in allFilters)
                AllFilters[filter.Name] = Delegate.CreateDelegate(typeof (LiquidFilter), filter);
        }

        public LiquidFilter this[string name]
        {
            get { return (LiquidFilter)AllFilters[name]; }
        }

        public void AddFilter(string name, LiquidFilter value)
        {
            AllFilters[name] = value;
        }
 
    }

    public class Utils
    {

        private static readonly Lazy<FilterFactory> AllFilters
            = new Lazy<FilterFactory>(() => new FilterFactory());


        private Utils()
        {
        }

        public static FilterFactory LiquidFilters { get { return AllFilters.Value; } }
 

        //Adds/changes a filter. WARNING: not synchronized, you should know what you're doing!
        [Obsolete("This method is not synchronized, and is applied on a singleton. You shouldn't be adding filters dynamically, unless you know very well what you're doing")]
        public void AddFilter(string name, LiquidFilter value)
        {
            LiquidFilters.AddFilter(name, value);
        }
    }
}
