namespace Chaos.Engine.Contracts
{
    public interface IUniversalController<out TClientSideController, out TServerSideController>
    {
        TClientSideController Client { get; }
        TServerSideController Server { get; }
    }
}