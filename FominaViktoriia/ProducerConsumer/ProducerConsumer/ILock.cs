namespace ProducerConsumer.Locks
{
    public interface ILock
    {
        void Lock();

        void Unlock();
    }
}
