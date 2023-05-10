package syncronizedset;

public interface ISynchronizedSet<T> {

	public boolean add(T value);
	
	public boolean remove(T value);
	
	public boolean contains(T value);
	
	public int size();

}
