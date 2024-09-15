namespace VIRTUE {
    public interface IGameEventListener<T> {
        void OnEventRaised (T item);
    }
}