using ProtoBuf;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Chaos.Engine.Network.Messages
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class CustomExplosionData
    {
        public BlockPos Position { get; set; }
        public EnumBlastType BlastType { get; set; }
        public double DestructionRadius { get; set; }
        public double InjureRadius { get; set; }
        public AssetLocation SoundFile { get; set; }
        public bool SmokeClouds { get; set; }
        public float PlayerDamage { get; set; }
        public bool SuppressItemDrops { get; set; }
    }
}