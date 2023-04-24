namespace ProducerConsumer.Locks
{
    public class TTASLock : ILock
    {
        private volatile int state = 0;

        public void Lock()
        {
            while (true)
            {
                while (state == 1) { }

                if (Interlocked.CompareExchange(ref state, 1, 0) == 0)
                {
                    return;
                }
            }
        }

        public void Unlock()
        {
            state = 0;
        }
    }
}
