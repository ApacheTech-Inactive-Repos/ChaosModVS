using Chaos.Engine.Network.Messages;

namespace Chaos.Engine.Contracts.Controllers
{
    public interface IServerGameworldController
    {
        void CreateExplosion(ExplosionData e);
    }
}