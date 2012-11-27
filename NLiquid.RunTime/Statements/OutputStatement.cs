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
using System.Collections.Generic;
using System.Linq;

namespace NLiquid.Runtime.Statements
{
    public class OutputStatement:IStatement
    {
        private readonly Expression _expr;
        private readonly String _outputStr;
        private readonly IList<FilterStatement> _filters;

        public OutputStatement(String str)
        {
            _outputStr = str;
        }

        public OutputStatement(Expression expr, StatementList filters)
        {
            _expr = expr;
            if(filters!= null)
            {
                _filters = new List<FilterStatement>();
                foreach (var filter in filters)
                {
                    if(filter is FilterStatement)
                        _filters.Add(filter as FilterStatement);
                    else
                        throw new ArgumentException(filter.Print(""),"filters");
                }

            }
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            if (_outputStr != null)
                return prefix + "OUTPUT: " + _outputStr.Replace("\n","\\n").Replace("\r","\\r");
            var retval = prefix + "OUTPUT: " + _expr.Print("");
            if(_filters!= null)
                retval = _filters.Aggregate(retval, (current, filter) => current + filter.Print(" "));
            return retval;
        }

        public object Execute(ILiquidContext context)
        {
            if(_outputStr != null)
                return _outputStr;
            var val = _expr.Evaluate(context);
            if (_filters != null)
                val = _filters.Aggregate(val, (current, filter) => filter.Evaluate(current));
            return val.ToString();
        }

        #endregion

    }
}
