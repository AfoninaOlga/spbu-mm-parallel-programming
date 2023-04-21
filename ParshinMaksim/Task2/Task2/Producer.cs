namespace Task2;

public class Producer<T> : IProducer<T>
{
	private readonly Func<T> produceFunc;
	private readonly Func<bool> canProduceFunc;

	public Producer(Func<T> produceFunc, Func<bool>? canProduceFunc = null)
	{
		this.produceFunc = produceFunc;
		this.canProduceFunc = canProduceFunc ?? (() => true);
	}

	public bool CanProduce => canProduceFunc();
	public T Produce() => produceFunc();
}
