package syncronizedset;

import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class Node<T> {

	private final int key;
	private final T value;
	private Node<T> next;
	private volatile boolean isMarked = false;
	private final Lock locker = new ReentrantLock();

	public Node(int key) {
		this.key = key;
		this.value = null;
	}

	public Node(int key, T value) {
		this.key = key;
		this.value = value;
	}

	public int getKey() {
		return key;
	}

	public Node<T> getNext() {
		return next;
	}

	public T getValue() {
		return value;
	}

	public boolean isMarked() {
		return isMarked;
	}

	public void lock() {
		locker.lock();
	}

	public void mark() {
		isMarked = true;
	}

	public void setNext(Node<T> next) {
		this.next = next;
	}

	public void unlock() {
		locker.unlock();
	}

}
