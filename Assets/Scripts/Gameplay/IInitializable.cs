namespace Gameplay
{
    public interface IInitializable
    {
        public int InitializationOrder { get; }
        public void Initialize(MissionManager manager);
    }
}