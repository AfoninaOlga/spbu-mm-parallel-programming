namespace Task2;

public interface IProducer<out T>
{
	public bool CanProduce { get; }
	public T Produce();
}
