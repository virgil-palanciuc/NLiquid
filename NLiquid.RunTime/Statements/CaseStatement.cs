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
    public class CaseStatement : IStatement
    {
        private readonly IList<KeyValuePair<Expression, StatementList>> _whenBranchStatements; 
        private StatementList _elseBranchStatements;
        private readonly Expression _conditionExpr;

        public CaseStatement(Expression condition)
        {
            if (condition == null)
                throw new ArgumentException("Invalid condition for CASE statement.");

            _conditionExpr = condition;
            _whenBranchStatements = new List<KeyValuePair<Expression, StatementList>>();
            _elseBranchStatements = null;
        }

        internal void AddWhenClause(Expression when, StatementList what)
        {
            _whenBranchStatements.Add(new KeyValuePair<Expression, StatementList>(when, what));
        }
        internal void AddElseClause(StatementList what)
        {
            _elseBranchStatements = what;
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            if (_elseBranchStatements != null)
                return string.Format("Case ({0}) Then\n {1} Else {2}", _conditionExpr.Print(prefix), String.Join("\n",_whenBranchStatements.Select(x => String.Format("WHEN {0}: {1}", x.Key.Print(prefix), x.Value.Print(prefix)))),
                                     _elseBranchStatements.Print(prefix));
            return string.Format("If ({0}) Then\n {1}", _conditionExpr.Print(prefix), String.Join("\n", _whenBranchStatements.Select(x => String.Format("WHEN {0}: {1}", x.Key.Print(prefix), x.Value.Print(prefix)))));
        }

        public object Execute(ILiquidContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
