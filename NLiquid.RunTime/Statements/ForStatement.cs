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
    public class ForStatement : IStatement
    {
        private readonly String _var;
        private readonly Expression _value;
        private readonly StatementList _attributes;
        private readonly StatementList _statements;

        public ForStatement(String var, Expression value, StatementList attributes, StatementList statements)
        {
            _var = var;
            _value = value;
            _attributes = attributes;
            _statements = statements;
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            return String.Format("{0}FOR {1} IN {2}\n{3}\n{4}", prefix, _var, _value.Print(""), _attributes.Print(prefix+": "), _statements.Print(prefix+"    "));
        }


        public object Execute(ILiquidContext context)
        {
            if(_attributes != null && _attributes.Count!=0)
                throw new NotImplementedException(); // todo: implement loop attributes

            if (!(_value is LookupExpression))
            {
                // TODO: implement FOR... in range
                throw new NotImplementedException();
                /*long initVal = (long)initExpr.Evaluate();
                long endVal = (long)endExpr.Evaluate();
                for (ielem.Value = initVal; ielem.Value <= endVal; ielem.Value++)
                {
                    statements.Execute();
                }*/
            }
            
            // FOR ... in collection
            var collection = _value.Evaluate(context) as IEnumerable<object>;
            if (collection != null)
            {
                var result =
                    collection.Select( item =>
                                           {
                                               context[_var] = item;
                                               return _statements.Execute(context);
                                           }
                                     ).Where(res => res != null);
                return result.Any() ? result.ToList() : null;
            }

            throw new NotImplementedException("not collection or range!");
        }

        #endregion
    }
}
