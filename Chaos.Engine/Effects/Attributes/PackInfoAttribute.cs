using System;

namespace Chaos.Engine.Effects.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class PackInfoAttribute : Attribute
    {
        public PackInfoAttribute()
        {
        }

        public PackInfoAttribute(string id)
        {
            Id = id;
        }

        public PackInfoAttribute(string id, string name)
        {
            (Id, Name) = (id, name);
        }

        public string Id { get; set; } = "default";

        public string Name { get; set; } = "Default";

        public string Description { get; set; } = "";

        public string Version { get; set; } = "0.1.0";

        public string Website { get; set; } = "";

        public string[] Authors { get; set; } = { };

        public string[] Contributors { get; set; } = { };

        public string[] Conflicts { get; set; } = { };

        public string[] Dependencies { get; set; } = { };
    }
}