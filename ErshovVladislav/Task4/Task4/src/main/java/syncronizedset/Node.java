package syncronizedset;

import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

public class Node<T> {

	private final int key;
	private T value;
	private Node<T> next;
	private final Lock locker = new ReentrantLock(); // TODO

	public Node(int key) {
		this.key = key;
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

	public void lock() {
		locker.lock();
	}

	public void setNext(Node<T> next) {
		this.next = next;
	}

	public void unlock() {
		locker.unlock();
	}

}
