namespace Task4
{
	public class FineGrainedSet<T> : ISet<T> where T : IComparable<T>
	{
		private readonly SetNode<T> head;
		private int count;

		public FineGrainedSet()
		{
			head = new();
			head.Next = new();
		}

		public int Count => count;

		public bool Add(T value)
		{
			head.Lock();
			var previous = head;
			try
			{
				var current = previous.Next;
				current.Lock();
				try
				{
					while (current.HasValue && current.Value.CompareTo(value) < 0)
					{
						previous.Unlock();
						previous = current;
						current = current.Next;
						current.Lock();
					}

					if (current.HasValue && current.Value.CompareTo(value) == 0)
					{
						return false;
					}

					var newNode = new SetNode<T>(value);
					newNode.Next = current;
					previous.Next = newNode;
					Interlocked.Increment(ref count);
					return true;
				}
				finally
				{
					current.Unlock();
				}
			}
			finally
			{
				previous.Unlock();
			}
		}

		public bool Remove(T value)
		{
			head.Lock();
			var previous = head;
			try
			{
				var current = previous.Next;
				current.Lock();
				try
				{
					while (current.HasValue && current.Value.CompareTo(value) < 0)
					{
						previous.Unlock();
						previous = current;
						current = current.Next;
						current.Lock();
					}

					if (current.HasValue && current.Value.CompareTo(value) == 0)
					{
						previous.Next = current.Next;
						Interlocked.Decrement(ref count);
						return true;
					}

					return false;
				}
				finally
				{
					current.Unlock();
				}
			}
			finally
			{
				previous.Unlock();
			}
		}

		public bool Contains(T value)
		{
			head.Lock();
			var previous = head;
			try
			{
				var current = previous.Next;
				current.Lock();
				try
				{
					while (current.HasValue && current.Value.CompareTo(value) < 0)
					{
						previous.Unlock();
						previous = current;
						current = current.Next;
						current.Lock();
					}

					return current.HasValue && current.Value.CompareTo(value) == 0;
				}
				finally
				{
					current.Unlock();
				}
			}
			finally
			{
				previous.Unlock();
			}
		}
	}
}
