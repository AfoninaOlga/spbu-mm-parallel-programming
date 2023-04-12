package main.java.ershov.consumer;

import java.util.Stack;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

/**
 * Consumer class.
 *
 * @author yersh2000@gmail.com
 */
public class Consumer implements Runnable {

    /** Consumer name. */
    private final String name;
    /** Consumer's thread. */
    private Thread thread;
    /** Extracted product. */
    private Object extractedProduct;
    /** Product buffer. */
    private final Stack<String> productBuffer;
    /** Lock and conditions for this consumer. */
    private final Lock lock;

    /**
     * Constructor for <code>Consumer</code> with product buffer and mutex.
     *
     * @param productBuffer product buffer where products are extracted from
     * @param lock          lock for this consumer
     */
    public Consumer(Stack<String> productBuffer, Lock lock) throws IllegalArgumentException {
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        } else if (lock == null) {
            throw new IllegalArgumentException("Lock can't be null");
        } else if (!(lock instanceof ReentrantLock)) {
            throw new IllegalArgumentException("Lock must be instance of ReentrantLock");
        }

        this.thread = new Thread(this, "consumer");
        this.name = "consumer_" + thread.getId();
        this.productBuffer = productBuffer;
        this.lock = lock;

        thread.start();
    }

    /**
     * Extract last produced product from productBuffer and put into extractedProduct.
     */
    private void extractProduct() {
        this.extractedProduct = productBuffer.pop();
    }

    @Override
    public void run() throws IllegalArgumentException {
        try {
            while (true) {
                lock.lock();

                if (!productBuffer.empty()) {
                    extractProduct();
                    System.out.println(name + " extracted: " + extractedProduct);
                }

                lock.unlock();
                Thread.sleep(1000);

                if (thread.isInterrupted()) {
                    throw new InterruptedException();
                }
            }
        } catch (InterruptedException e) {
            System.out.println(name + " finished");
        }
    }

    /** Stop consumer. */
    public void stop() {
        thread.interrupt();
    }

}
