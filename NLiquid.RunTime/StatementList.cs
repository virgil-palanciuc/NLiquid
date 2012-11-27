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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLiquid.Runtime
{
    public class StatementList : IStatement, IEnumerable<IStatement>
    {
		internal StatementList() {} // prevent construction from oustide the package

        public void Add(IStatement statement)
        {
            _statements.Add(statement);
        }

        public void InsertFront(IStatement statement)
        {
            _statements.Insert(0, statement);
        }

        #region IStatement Members

        public string Print(string prefix)
        {
            var str = new StringBuilder();
            foreach (var st in _statements)
                str.AppendLine(st.Print(prefix));

            return str.ToString();
        }

        public object Execute(ILiquidContext context)
        {
            var l = _statements.Where(st => st != null).Select(st => st.Execute(context)).Where(res => res != null);
            return l.Any() ? l.ToList() : null;
        }

        #endregion

        private readonly List<IStatement> _statements = new List<IStatement>();
        public int Count
        {
            get { return _statements.Count; }
        }

        public IEnumerator<IStatement> GetEnumerator()
        {
            return _statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
