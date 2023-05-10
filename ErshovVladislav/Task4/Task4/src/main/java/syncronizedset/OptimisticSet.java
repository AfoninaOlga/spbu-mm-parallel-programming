package syncronizedset;

public class OptimisticSet<T> implements ISynchronizedSet<T> {

	private Node<T> head = new Node<>(Integer.MIN_VALUE);
	private Node<T> tail = new Node<>(Integer.MAX_VALUE);

	public OptimisticSet() {
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
						return current.getKey() == key;
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
	public int size() {
		Node<T> current = head.getNext();
		while (current != tail) {
			current.lock();
			current = current.getNext();
		}

		try {
			int count = 0;
			while (current != tail) {
				count++;
				current = current.getNext();
			}

			return count;
		} finally {
			current = head.getNext();
			while (current != tail) {
				current.unlock();
				current = current.getNext();
			}
		}
	}

	private boolean validate(Node<T> previous, Node<T> current) {
		Node<T> node = head;
		while (node.getKey() < previous.getKey()) {
			if (node.getKey() == previous.getKey()) {
				return previous.getNext().getKey() == current.getKey();
			}

			node = node.getNext();
		}

		return false;
	}

}
