using System.Collections.Generic;

namespace BOM.CORE
{
    public interface IBScriptParser
    {
        IEnumerable<BScriptParseResult> Parse(string BomScript);
    }
}