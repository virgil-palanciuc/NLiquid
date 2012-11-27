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

namespace NLiquid.Runtime.Statements
{
    public class IncludeStatement : IStatement
    {
        #region Constructor

        public IncludeStatement(String template, Expression withExpr)
        {
            this.template = template;
            this.withExpr = withExpr;
        }

        #endregion

        #region Private Fields

        String template = null;
        Expression withExpr = null;

        #endregion

        #region IStatement Members

        public string Print(string prefix)
        {
            if(withExpr == null)
                return String.Format("Include {0}", template);
            return  String.Format("Include {0} With {1}", template, withExpr.Print(prefix) );
        }

        public object Execute(ILiquidContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
