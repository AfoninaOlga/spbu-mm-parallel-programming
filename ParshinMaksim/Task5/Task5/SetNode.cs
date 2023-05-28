namespace Task5
{
	internal class SetNode<V>
	{
		private readonly V value = default;

		public bool HasValue { get; private set; }

		public V Value
		{
			get
			{
				if (HasValue)
				{
					return value;
				}

				throw new InvalidOperationException();

			}
		}

		public bool IsMarked { get; set; }

		public SetNode<V> Next { get; set; }

		private Mutex mutex = new(false);

		public SetNode(V value)
		{
			this.value = value;
			HasValue = true;
			Next = this;
		}

		public SetNode()
		{
			Next = this;
		}

		public void Lock() => mutex.WaitOne();

		public void Unlock() => mutex.ReleaseMutex();
	}
}
