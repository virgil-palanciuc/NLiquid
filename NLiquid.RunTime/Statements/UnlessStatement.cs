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
    public class UnlessStatement:IStatement
    {
        private readonly StatementList _unlessBranchStatements;
        private StatementList _elseBranchStatements;
        private readonly Expression _conditionExpr;

        public UnlessStatement(Expression condition, StatementList unlessBranch,
                            StatementList elseBranch)
        {
            if (condition == null)
            {
                throw new ArgumentException("Invalid condition for IF statement.");
            }
            _conditionExpr = condition;

            _unlessBranchStatements = unlessBranch;
            _elseBranchStatements = elseBranch;
        }

        public void AddElseClause(StatementList elseBranch)
        {
            _elseBranchStatements = elseBranch;
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            if (_elseBranchStatements != null)
                return string.Format("Unless ({0}) Then\n {1} Else {2}", _conditionExpr.Print(prefix), _unlessBranchStatements.Print(prefix),
                                     _elseBranchStatements.Print(prefix));
            return string.Format("Unless ({0}) Then\n {1}", _conditionExpr.Print(prefix), _unlessBranchStatements.Print(prefix) );
        }

        public object Execute(ILiquidContext context)
        {
            var result = (bool)_conditionExpr.Evaluate(context);
            return !result ? _unlessBranchStatements.Execute(context) : _elseBranchStatements.Execute(context);
        }

        #endregion
         
    }
}