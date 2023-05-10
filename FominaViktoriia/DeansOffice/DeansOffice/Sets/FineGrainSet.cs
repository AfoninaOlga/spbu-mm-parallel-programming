using DeansOffice.ExamSystemLogic;

namespace DeansOffice.Sets
{
    public class FineGrainedSet : IExamSystem
    {
        private readonly Node<(long, long)> head;
        private volatile int count;

        public FineGrainedSet()
        {
            head = new Node<(long, long)>(int.MinValue);
            head.Next = new Node<(long, long)>(Int32.MaxValue);
        }

        public void Add(long studentId, long courseId)
        {
            var item = (studentId, courseId);
            int key = item.GetHashCode();
            head.Lock.Lock();
            Node<(long, long)> pred = head;
            try
            {
                Node<(long, long)> curr = pred.Next;
                curr.Lock.Lock();
                try
                {
                    while (curr.Key < key)
                    {
                        pred.Lock.Unlock();
                        pred = curr;
                        curr = curr.Next;
                        curr.Lock.Lock();
                    }
                    if (curr.Key == key)
                    {
                        return;
                    }
                    var newNode = new Node<(long, long)>(item)
                    {
                        Next = curr
                    };
                    pred.Next = newNode;
                    ++count;
                    return;
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

        public void Remove(long studentId, long courseId)
        {
            var item = (studentId, courseId);
            Node<(long, long)> pred = null, curr = null;
            int key = item.GetHashCode();
            head.Lock.Lock();
            try
            {
                pred = head;
                curr = pred.Next;
                curr.Lock.Lock();
                try
                {
                    while (curr.Key < key)
                    {
                        pred.Lock.Unlock();
                        pred = curr;
                        curr = curr.Next;
                        curr.Lock.Lock();
                    }
                    if (curr.Key == key)
                    {
                        pred.Next = curr.Next;
                        --count;
                        return;
                    }
                    return;
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

        public bool Contains(long studentId, long courseId)
        {
            var item = (studentId, courseId);
            Node<(long, long)> pred = null, curr = null;
            int key = item.GetHashCode();
            head.Lock.Lock();
            try
            {
                pred = head;
                curr = pred.Next;
                curr.Lock.Lock();
                try
                {
                    while (curr.Key < key)
                    {
                        pred.Lock.Unlock();
                        pred = curr;
                        curr = curr.Next;
                        curr.Lock.Lock();
                    }
                    return curr.Key == key;
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

        public int Count => count;
    }
}
