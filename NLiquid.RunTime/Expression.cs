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

namespace NLiquid.Runtime
{
    public enum Operation
    {
        Constant,
        And,
        Or,
        Not,
        Equ,
        NotEqu,
        Lt,
        Gt,
        LtEq,
        GtEq,
        Lookup,
        Range
    }

    public class LookupExpression: Expression
    {
        private readonly List<String> _fragments;

        public LookupExpression(String name):base(Operation.Lookup)
        {
            _fragments = new List<String> {name};
        }
        public void AddMemberRef(String member)
        {
            _fragments.Add(member);
        } 

        public override string Print(string prefix)
        {
            return String.Format("{0}${1}", prefix, String.Join(".",_fragments) );
        }

        public override Object Evaluate(ILiquidContext context)
        {
            if (_fragments==null || _fragments.Count == 0)
                return null;

            var val = context[_fragments[0]];

            if(_fragments.Count == 1)
                return val;

            foreach(var member in _fragments.Skip(1))
            {
                var myObject = val as IDictionary<String, object>;
                if(myObject == null)
                    return null;
                val = myObject[member];
            }
            return val;
        }
    }

    public class ConstExpression: Expression
    {
        private readonly object _constValue;
         public ConstExpression(long value):base(Operation.Constant)
        {
            _constValue = value;
        }

        public ConstExpression():base(Operation.Constant)
        {
            _constValue = null; // "nil" literal
        }

        public ConstExpression(bool value):base(Operation.Constant)
        {
            _constValue = value;
        }

        public ConstExpression(double value):base(Operation.Constant)
        {
            _constValue = value;
        }

        public ConstExpression(string value):base(Operation.Constant)
        {
            _constValue = value;
        }

        public override string Print(string prefix)
        {
            return prefix+_constValue;
        }

        public override Object Evaluate(ILiquidContext context)
        {
            return _constValue;
        }
       
    }

    public class BinaryExpression : Expression
    {
        private readonly Expression _leftExpression;
        private readonly Expression _rightExpression;
        
        /// <summary>
        /// Builds a complex expression.
        /// </summary>
        /// <param name="op">Op</param>
        /// <param name="left">Left side expression.</param>
        /// <param name="right">Right side expression.</param>
        public BinaryExpression(Operation op, Expression left, Expression right):base(op)
        {
            if (right == null || left == null)
                throw new ArgumentNullException(left==null?"left":"right");
            _leftExpression = left;
            _rightExpression = right;
        }

        public override string Print(string prefix)
        {
            return prefix + String.Format("( {0} {1} {2} )", _leftExpression.Print(prefix), Op, _rightExpression.Print(prefix));
        }

        public override Object Evaluate(ILiquidContext context)
        {
            switch(Op)
            {
                case Operation.Gt:
                    {
                        var left = _leftExpression.Evaluate(context) as IComparable;
                        var right = _rightExpression.Evaluate(context) as IComparable;
                        return left != null && left.CompareTo(right) > 0;
                    }
                case Operation.Lt:
                    {
                        var left = _leftExpression.Evaluate(context) as IComparable;
                        var right = _rightExpression.Evaluate(context) as IComparable;
                        return left != null && left.CompareTo(right) < 0;
                    }
                case Operation.LtEq:
                    {
                        var left = _leftExpression.Evaluate(context) as IComparable;
                        var right = _rightExpression.Evaluate(context) as IComparable;
                        return left != null && left.CompareTo(right) <= 0;
                    }
                case Operation.GtEq:
                    {
                        var left = _leftExpression.Evaluate(context) as IComparable;
                        var right = _rightExpression.Evaluate(context) as IComparable;
                        return left != null && left.CompareTo(right) >= 0;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

    }

    public abstract class Expression:IStatement
    {
        protected Expression(Operation op)
        {
            Op = op; 
        }

        public Operation Op { get; private set; }


        public object Execute(ILiquidContext context)
        {
            Evaluate(context);
            return null;
        }

        public abstract string Print(string prefix);
        public abstract object Evaluate(ILiquidContext context);
    }
}
