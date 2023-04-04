package main.java.consumer;

import java.util.Stack;
import java.util.concurrent.Semaphore;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.Lock;

/**
 * Consumer class.
 *
 * @author yersh2000@gmail.com
 */
public class Consumer implements Runnable {

    /** Consumer name. */
    private final String name;
    /** Consumer stop checking flag. */
    private boolean isConsumerStopped = false;
    /** Extracted product. */
    private Object extractedProduct;
    /** Product buffer. */
    private final Stack<String> productBuffer;
    /** Lock and conditions for this consumer. */
    private final Lock lock;
    private final Condition notFull;
    private final Condition notEmpty;

    /**
     * Constructor for <code>Consumer</code> with product buffer and mutex.
     *
     * @param productBuffer product buffer where products are extracted from
     * @param lock          lock for this consumer
     */
    public Consumer(Stack<String> productBuffer, Lock lock, Condition notFull, Condition notEmpty) throws IllegalArgumentException {
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        } else if (lock == null || notFull == null || notEmpty == null) {
            throw new IllegalArgumentException("Lock or conditions can't be null");
        }

        Thread thread = new Thread(this, "consumer");
        this.name = "consumer_" + thread.getId();
        this.productBuffer = productBuffer;
        this.lock = lock;
        this.notFull = notFull;
        this.notEmpty = notEmpty;

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
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        } else if (lock == null || notFull == null || notEmpty == null) {
            throw new IllegalArgumentException("Lock or conditions can't be null");
        }

        try {
            while (true) {
                lock.lock();

                if (!productBuffer.empty()) {
                    extractProduct();
                    System.out.println(name + " extracted: " + extractedProduct);
                }

                lock.unlock();
                Thread.sleep(1000);

                if (isConsumerStopped) {
                    throw new InterruptedException();
                }
            }
        } catch (InterruptedException e) {
            System.out.println(name + " finished");
        }
    }

    /** Stop consumer. */
    public void stop() {
        isConsumerStopped = true;
    }

}
