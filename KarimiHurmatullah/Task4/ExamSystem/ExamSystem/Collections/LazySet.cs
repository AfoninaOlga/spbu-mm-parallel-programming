namespace ExamSystem.Collections
{
    public class LazySet<T>
    {
        private readonly Node<T> tail = new(int.MaxValue);
        private readonly Node<T> head = new(int.MinValue);
        private int count;

        public LazySet()
        {
            head.Next = tail;
        }

        private static bool Validate(Node<T> pred, Node<T> curr)
        {
            return !pred.Marked && !curr.Marked && pred.Next == curr;
        }

        public void Add(T item)
        {
            int key = item.GetHashCode();
            while (true)
            {
                Node<T> pred = head;
                Node<T> curr = head.Next;
                while (curr.Key < key)
                {
                    pred = curr;
                    curr = curr.Next;
                }
                pred.Lock.Lock();
                try
                {
                    curr.Lock.Lock();
                    try
                    {
                        if (Validate(pred, curr))
                        {
                            if (curr.Key == key)
                            {
                                return;
                            }
                            else
                            {
                                Node<T> node = new Node<T>(item) { Next = curr };
                                pred.Next = node;
                                Interlocked.Increment(ref count);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        curr.Lock.Unlock();
                    }
                }
                finally
                {
                    pred.Lock.Unlock();
                }
            }
        }

        public void Remove(T item)
        {
            int key = item.GetHashCode();
            while (true)
            {
                Node<T> pred = head;
                Node<T> curr = head.Next;
                while (curr.Key < key)
                {
                    pred = curr;
                    curr = curr.Next;
                }
                pred.Lock.Lock();
                try
                {
                    curr.Lock.Lock();
                    try
                    {
                        if (Validate(pred, curr))
                        {
                            if (curr.Key != key)
                            {
                                return;
                            }
                            else
                            {
                                curr.Marked = true;
                                pred.Next = curr.Next;
                                Interlocked.Decrement(ref count);
                                return;
                            }
                        }
                    }
                    finally
                    {
                        curr.Lock.Unlock();
                    }
                }
                finally
                {
                    pred.Lock.Unlock();
                }
            }
        }

        public bool Contains(T item)
        {
            int key = item.GetHashCode();
            Node<T> curr = head;
            while (curr.Key < key)
                curr = curr.Next;
            return curr.Key == key && !curr.Marked;
        }

        public int Count => count;
    }

}
