using System.Collections;

namespace NLiquid.Runtime.FilterDefinitions
{
    public class StandardFilters
    {
        public static object PrettyPrint(object input, IEnumerable filterArguments)
        {
            return "pretty ----> " + input + " ----> text";
        }
    }
}
