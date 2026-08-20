namespace saper1.IServices
{
    public interface IGameTimer
    {
        event Action<int, int> TimeChanged;
        void Start();
        void Stop();
        void Reset();
    }
}
