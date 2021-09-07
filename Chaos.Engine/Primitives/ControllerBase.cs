using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Engine.Primitives
{
    public abstract class ControllerBase<TServerSideAPI, TClientSideAPI> 
        where TServerSideAPI : new() 
        where TClientSideAPI : new()
    {
        protected static ICoreAPI Api { get; private set;  }
        protected static ICoreClientAPI Capi { get; private set; }
        protected static ICoreServerAPI Sapi { get; private set; }

        protected ControllerBase(ICoreAPI api)
        {
            Api = api;
            Capi = api as ICoreClientAPI;
            Sapi = api  as ICoreServerAPI;
        }


        public TServerSideAPI Server => new();
        public TClientSideAPI Client => new();
    }
}