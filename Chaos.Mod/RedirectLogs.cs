#if DEBUG
using System.Diagnostics;
using Vintagestory.API.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Chaos.Mod
{
    /// <summary>
    ///     Redirects all log entries into the visual studio output window. Only for your convenience during development and testing.
    /// </summary>
    public class RedirectLogs : ModSystem
    {
        public override double ExecuteOrder() => 0.0;

        public override bool ShouldLoad(EnumAppSide side) => true;

        public override void Start(ICoreAPI api)
        {
            [DebuggerHidden]
            void OnLoggerOnEntryAdded(EnumLogType logType, string message, object[] args)
            {
                if (logType == EnumLogType.VerboseDebug) return;
                Debug.WriteLine($"[{api.Side} {logType}] {message}", args);
            }
            api.World.Logger.EntryAdded += OnLoggerOnEntryAdded;
        }
    }
}
#endif