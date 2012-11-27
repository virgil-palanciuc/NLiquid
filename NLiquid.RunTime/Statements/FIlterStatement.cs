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

namespace NLiquid.Runtime.Statements
{
    public class FilterStatement : IStatement
    {
        private readonly Func<object, IEnumerable, object> _filter;
        private readonly string _filterName;
        private readonly List<Expression> _arguments;

        public FilterStatement(String filterName, StatementList args)
        {
            _filterName = filterName;
            _filter = Utils.LiquidFilters[filterName];
            if (args != null && args.Count > 0)
            {
                _arguments = new List<Expression>();
                foreach (var statement in args)
                    if (statement is Expression)
                        _arguments.Add((Expression) statement);
                    else
                        throw new ArgumentException(String.Format("Wrong argument to filter: {0}", statement.Print("")));
            }
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            if (_arguments == null || _arguments.Count == 0)
                return prefix + "| " + _filterName;
            return prefix +
                   String.Format("| {0} :{1}", _filterName, String.Join(",", _arguments.ConvertAll(e => e.Print(""))));
        }

        public object Execute(ILiquidContext context)
        {
            throw new NotSupportedException("Filter "+_filterName+" can't be executed directly");
        }

        public object Evaluate(object onValue)
        {
            return _filter(onValue, _arguments); 
        }

        #endregion

    }
}