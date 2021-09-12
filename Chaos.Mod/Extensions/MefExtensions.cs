using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;

namespace Chaos.Mod.Extensions
{
    public static class MefExtensions
    {
        public static IEnumerable<Assembly> GetAssembliesWithExports(this CompositionContainer container)
        {
            return container.Catalog?.Parts
                .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                .Distinct()
                .ToList();
        }
    }
}