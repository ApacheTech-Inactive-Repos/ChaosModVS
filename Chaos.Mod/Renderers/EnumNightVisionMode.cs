using System;
using JetBrains.Annotations;

namespace Chaos.Mod.Renderers
{
    [Flags]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum EnumNightVisionMode
    {
        FilterNone,
        FilterSepia,
        FilterGray,
        FilterGreen = 4,
        FilterBlue = 8,
        FilterRed = 16,
        Deactivated = 32,
        Filter = 63,
        Compress,
        Default
    }
}