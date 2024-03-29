﻿/********************************************************************************
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

using System.Collections.Generic;

namespace NLiquid.Runtime
{
    public interface ILiquidContext
    {
        object this[string name] { get; set; }
        ILiquidContext Clone();
    }

    public class DefaultSymbolTable: ILiquidContext 
    {
        #region Private Fields

        private readonly SortedDictionary<string, object> _symbolTable;
        #endregion

        public DefaultSymbolTable()
        {
            _symbolTable = new SortedDictionary<string, object>();
        }

        #region Public Methods

        public object this[string name]
        {
            get
            { 
                object val;
                _symbolTable.TryGetValue(name, out val);
                return val;
            }
            set { _symbolTable[name] = value; }
        }
        #endregion


        public ILiquidContext Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
