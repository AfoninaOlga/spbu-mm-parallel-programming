namespace Task5
{
	public interface ISet<T>
	{
		bool Add(T value);

		bool Remove(T value);

		bool Contains(T value);

		int Count { get; }
	}
}
