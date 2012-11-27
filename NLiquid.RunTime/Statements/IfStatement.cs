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
    public class IfStatement : IStatement
    {
        private readonly StatementList _ifBranchStatements;
        private StatementList _elseBranchStatements;
        private readonly Expression _conditionExpr;

        //bool hasElseBranch = false;
        ///// <summary>
        ///// Property indicating wether ther is an else branch.
        ///// </summary>
        //public bool HasElseBranch {
        //    get { return this.hasElseBranch; }
        //    set { this.hasElseBranch = value; }
        //}

        public IfStatement(Expression condition, StatementList ifBranch,
                            StatementList elseBranch = null)
        {
            if (condition == null)
                throw new ArgumentException("Invalid condition for IF statement.");
            _conditionExpr = condition;

            _ifBranchStatements = ifBranch;
            _elseBranchStatements = elseBranch;
        }
        internal void AddElseClause(StatementList what)
        {
            _elseBranchStatements = what;
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            if (_elseBranchStatements != null)
                return string.Format("If ({0}) Then\n {1} Else {2}", _conditionExpr.Print(prefix), _ifBranchStatements.Print(prefix),
                                     _elseBranchStatements.Print(prefix));
            return string.Format("If ({0}) Then\n {1}", _conditionExpr.Print(prefix), _ifBranchStatements.Print(prefix) );
        }

        public object Execute(ILiquidContext context)
        {
            var result = (bool)_conditionExpr.Evaluate(context);
            return result ? _ifBranchStatements.Execute(context) : 
                _elseBranchStatements != null? _elseBranchStatements.Execute(context): null;
        }

        #endregion
    }
}
