using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Util;

namespace Chaos.Mod
{
    public static class StringEx
    {
        public static bool StartsWithAny(this string str, IEnumerable<string> matches)
        {
            return matches.Any(str.StartsWithFast);
        }
        public static bool StartsWithAny(this string str, params string[] matches)
        {
            return matches.Any(str.StartsWithFast);
        }
    }
}