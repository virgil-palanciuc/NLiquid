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
using System.Text;
using System.Threading.Tasks;
using NLiquid.Analyzing;
using NLiquid.Runtime;

namespace NLiquid.Interpreter
{
    class Program
    {
        static StringBuilder BuildResult(StringBuilder sb, object result)
        {
            if (result == null) return sb;

            if (result is IEnumerable<object>)
            {
                var collection = result as IEnumerable<object>;
                collection.Aggregate(sb, BuildResult);
                return sb;
            }

            if(result is Task<object>)
            {
                var task = result as Task<object>;
                task.Wait();
                return BuildResult(sb, task.Result);
            }

            return sb.Append(result.ToString());
        }

        static void Main()
        {
            const string test = "<ul id=\"products\">\n" +
                                "  {% for product in products %}\n" +
                                "    <li>\n" +
                                "      <h2>{{ product.name }}</h2>\n" +
                                "      {%if product.price< 10 %} Only {%else%} Price: {%endif%} {{ product.price }}\n" +
                                "\n" +
                                "      {{ product.description | prettyPrint }}\n" +
                                "    </li>\n" +
                                "  {% endfor %}\n" +
                                "  \n" +
                                "  {% timeout 500 %}\n" +
                                "  \n" +
                                "  {% timeout 100 %}\n" +
                                "  \n" +
                                "  {% timeout 200 %}\n" +
                                "</ul>";
            var scn = new Scanner();
            scn.SetSource(test, 0);
            var parser = new Parser(scn, new StatementFactory());
            parser.Parse();
            Console.WriteLine(parser.program.Print(""));
            Console.WriteLine();
            Console.WriteLine("===================================================================================================");
            Console.WriteLine();

            var products = new SortedDictionary<String, object>[10];
            for(var idx=0; idx<10;idx++)
                products[idx] = ((Func<int,SortedDictionary<String,object> >)
                                 (i => new SortedDictionary<String,object>
                                        {
                                            { "id", i },
                                            { "name", "Product"+(i+1) },
                                            { "price", i+7.50 },
                                            { "description", String.Format("Super product {0} description",i+1) }
                                        } ))(idx);

            var st = new DefaultSymbolTable();
            st["products"] = products;
            var result = parser.program.Execute(st);
             Console.WriteLine(BuildResult(new StringBuilder(), result).ToString());
        }
    }
}
