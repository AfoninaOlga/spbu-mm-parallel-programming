
namespace Task4
{
	public class LazySet<T> : ISet<T> where T : IComparable<T>
	{
		private readonly SetNode<T> head;
		private int count;

		public LazySet()
		{
			head = new();
			head.Next = new();
		}
		public int Count => count;

		private bool Validate(SetNode<T> previous, SetNode<T> current) =>
			!previous.IsMarked && !current.IsMarked && previous.Next == current;

		public bool Add(T value)
		{
			while (true)
			{
				var previous = head;
				var current = head.Next;

				while (current.HasValue && current.Value.CompareTo(value) < 0)
				{
					previous = current;
					current = current.Next;
				}

				previous.Lock();
				current.Lock();

				try
				{
					if (Validate(previous, current))
					{
						if (current.HasValue && current.Value.CompareTo(value) == 0)
						{
							return false;
						}

						var node = new SetNode<T>(value) { Next = current };
						previous.Next = node;
						Interlocked.Increment(ref count);
						return true;
					}
				}
				finally
				{
					current.Unlock();
					previous.Unlock();
				}

			}
		}

		public bool Remove(T value)
		{
			while (true)
			{
				var previous = head;
				var current = head.Next;

				while (current.HasValue && current.Value.CompareTo(value) < 0)
				{
					previous = current;
					current = current.Next;
				}

				previous.Lock();
				current.Lock();

				try
				{
					if (Validate(previous, current))
					{
						if (!current.HasValue || current.Value.CompareTo(value) != 0)
						{
							return false;
						}

						current.IsMarked = true;
						previous.Next = current.Next;
						Interlocked.Decrement(ref count);
						return true;
					}
				}
				finally
				{
					current.Unlock();
					previous.Unlock();
				}
			}
		}

		public bool Contains(T value)
		{
			var current = head.Next;

			while (current.HasValue && current.Value.CompareTo(value) < 0)
			{
				current = current.Next;
			}

			return current.HasValue && current.Value.CompareTo(value) == 0 && !current.IsMarked;
		}
	}
}
