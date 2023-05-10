package syncronizedset;

public class LazySet<T> implements ISynchronizedSet<T> {

	private Node<T> head = new Node<>(Integer.MIN_VALUE);
	private Node<T> tail = new Node<>(Integer.MAX_VALUE);

	public LazySet() {
		head.setNext(tail);
	}

	@Override
	public boolean add(T value) {
		int key = value.hashCode();
		while (true) {
			Node<T> previous = head;
			Node<T> current = head.getNext();

			while (current.getKey() < key) {
				previous = current;
				current = current.getNext();
			}

			previous.lock();
			try {
				current.lock();
				try {
					if (validate(previous, current)) {
						if (current.getKey() == key) {
							return false;
						} else {
							Node<T> node = new Node<T>(key, value);
							node.setNext(current);
							previous.setNext(node);

							return true;
						}
					}
				} finally {
					current.unlock();
				}
			} finally {
				previous.unlock();
			}
		}
	}

	@Override
	public boolean remove(T value) {
		int key = value.hashCode();
		while (true) {
			Node<T> previous = head;
			Node<T> current = head.getNext();

			while (current.getKey() < key) {
				previous = current;
				current = current.getNext();
			}

			previous.lock();
			try {
				current.lock();
				try {
					if (validate(previous, current)) {
						if (current.getKey() != key) {
							return false;
						} else {
							current.mark();
							previous.setNext(current.getNext());
							return true;
						}
					}
				} finally {
					current.unlock();
				}
			} finally {
				previous.unlock();
			}
		}
	}

	@Override
	public boolean contains(T value) {
		int key = value.hashCode();
		Node<T> current = head;

		while (current.getKey() < key) {
			current = current.getNext();
		}

		return current.getKey() == key && !current.isMarked();
	}

	@Override
	public int size() {
		Node<T> current = head;
		while (!current.equals(tail)) {
			current.lock();
			current = current.getNext();
		}
		current.lock();

		try {
			int count = 0;
			current = head.getNext();
			while (!current.equals(tail)) {
				count++;
				current = current.getNext();
			}

			return count;
		} finally {
			current = head;
			while (!current.equals(tail)) {
				current.unlock();
				current = current.getNext();
			}
			current.unlock();
		}
	}

	private boolean validate(Node<T> previous, Node<T> current) {
		return !previous.isMarked() && !current.isMarked() && previous.getNext().equals(current);
	}

}
