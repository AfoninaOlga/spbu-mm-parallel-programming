namespace Task2;

public interface IConsumer<in T>
{
	public void Consume(T value);
}
