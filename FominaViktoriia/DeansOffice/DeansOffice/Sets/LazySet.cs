using DeansOffice.ExamSystemLogic;

namespace DeansOffice.Sets
{
    public class LazySet : IExamSystem
    {
        private readonly Node<(long, long)> _tail = new(int.MaxValue);
        private readonly Node<(long, long)> _head = new(int.MinValue);
        private volatile int count;

        public LazySet()
        {
            _head.Next = _tail;
        }

        private static bool Validate(Node<(long, long)> pred, Node<(long, long)> curr)
        {
            return !pred.Marked && !curr.Marked && pred.Next == curr;
        }

        public void Add(long studentId, long courseId)
        {
            var item = (studentId, courseId);
            int key = item.GetHashCode();
            while (true)
            {
                Node<(long, long)> pred = _head;
                Node<(long, long)> curr = _head.Next;
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
                                Node<(long, long)> node = new Node<(long, long)>(item) { Next = curr };
                                pred.Next = node;
                                ++count;
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

        public void Remove(long studentId, long courseId)
        {
            var item = (studentId, courseId);
            int key = item.GetHashCode();
            while (true)
            {
                Node<(long, long)> pred = _head;
                Node<(long, long)> curr = _head.Next;
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
                                --count;
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

        public bool Contains(long studentId, long courseId)
        {
            var item = (studentId, courseId);
            int key = item.GetHashCode();
            Node<(long, long)> curr = _head;
            while (curr.Key < key)
                curr = curr.Next;
            return curr.Key == key && !curr.Marked;
        }

        public int Count => count;
    }
}
