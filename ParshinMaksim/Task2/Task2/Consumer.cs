namespace Task2;

public class Consumer<T> : IConsumer<T>
{
	private readonly Action<T> consumeAction;

	public Consumer(Action<T> consumeAction)
	{
		this.consumeAction = consumeAction;
	}

	public void Consume(T value) => consumeAction(value);
}
