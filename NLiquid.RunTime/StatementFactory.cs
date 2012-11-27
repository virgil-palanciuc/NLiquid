using System;
using System.Diagnostics;
using NLiquid.Runtime.Statements;
using Attribute = NLiquid.Runtime.Statements.Attribute;

namespace NLiquid.Runtime
{

    public class StatementFactory
	{
		public virtual StatementList NewList() { return new StatementList(); }
        public virtual IStatement NewAttribute(String name, Expression value)
        {
            return new Attribute(name, value);
        }
        public virtual IStatement NewFilter(String name, StatementList args)
        {
            return new FilterStatement(name, args);
        }
        public virtual IStatement NewAssignment(String name, Expression value)
        {
            return new AssignmentStatement(name, value);
        }
        public virtual IStatement Write(Expression value, StatementList filters = null)
        {
            return new OutputStatement(value, filters);
        }
        public virtual IStatement Write(String value)
        {
            return new OutputStatement(value);
        }
        public virtual IStatement NewCycle(string group, StatementList statements)
        {
            return new CycleStatement(group, statements);
        }
        public virtual IStatement NewForLoop(string var, Expression value, StatementList attributes, StatementList statements)
        {
            return new ForStatement(var, value, attributes, statements);
        }

        public virtual IStatement NewCapture(string var, StatementList statements )
        {
            return  new CaptureStatement(var, statements);
        }

        public virtual IStatement NewCase(Expression expr)
        {
            return new CaseStatement(expr);
        }

        public virtual IStatement NewIf(Expression expr, StatementList thenBranch, StatementList elseBranch = null)
        {
            return new IfStatement(expr, thenBranch, elseBranch);
        }

        public virtual IStatement NewUnless(Expression expr, StatementList thenBranch, StatementList elseBranch = null)
        {
            return new UnlessStatement(expr, thenBranch, elseBranch);
        }

        public virtual IStatement NewTimeout(double timeout)
        {
            return new TimeoutStatement(timeout);
        }

        public virtual IStatement NewTableRow(string var, Expression value, StatementList attributes, StatementList statements)
        {
            return new TableRowStatement(var, value, attributes, statements);
        }

        public virtual void AddWhenClause(IStatement caseInst, Expression when, StatementList what)
        {
            var cs = caseInst as CaseStatement;
            Debug.Assert(cs != null);
            cs.AddWhenClause(when, what);
        }

        public virtual void AddElseClause(IStatement inst, StatementList what)
        {
            var cs = inst as CaseStatement;
            if (cs != null)
            {
                cs.AddElseClause(what);
                return;
            }

            var ifInst = inst as IfStatement;
            if( ifInst  != null)
            {
                ifInst.AddElseClause(what);
                return;
            }

            Debug.Assert(inst is UnlessStatement);

            var unlessInst = inst as UnlessStatement;
            unlessInst.AddElseClause(what);
        }


        public virtual IStatement Include(string file, Expression withExpr)
        {
            return new IncludeStatement(file, withExpr);
        }

        public virtual Expression NewExpression(Operation op, Expression left, Expression right)
        {
            return new BinaryExpression(op, left, right);
        }
        public virtual Expression NewExpression()
        {
            return new ConstExpression();
        }
        public virtual Expression NewExpression(int value)
        {
            return new ConstExpression(value);
        }
        public virtual Expression NewExpression(bool value)
        {
            return new ConstExpression(value);
        }
        public virtual Expression NewExpression(double value)
        {
            return new ConstExpression(value);
        }
        public virtual Expression NewExpression(string value)
        {
            return new ConstExpression(value);
        }
        public virtual Expression NewExpression(long value)
        {
            return new ConstExpression(value);
        }
        public virtual Expression NewExpression(Operation op, string name)
        {
            Debug.Assert(op == Operation.Lookup);
            return new LookupExpression(name);
        }
        public virtual Expression NewExpression(Operation op, Expression left, string fragment)
        {
            Debug.Assert(op == Operation.Lookup);
            var expr = left as LookupExpression;
            Debug.Assert(expr != null);
            expr.AddMemberRef(fragment);
            return expr;
        }
    }
}

