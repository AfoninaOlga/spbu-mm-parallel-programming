package main.java.producer;

import java.util.Stack;
import java.util.concurrent.locks.Lock;

/**
 * Producer class.
 *
 * @author yersh2000@gmail.com
 */
public class Producer implements Runnable {

    /** Producer name. */
    private final String name;
    /** Producer stop checking flag. */
    private boolean isProducerStopped = false;
    /** Producer product. */
    private String producedProduct;
    /** Number of products produced. */
    private Integer numberProducedProduct = 0;
    /** Product buffer. */
    private final Stack<String> productBuffer;
    /** Lock for this producer. */
    private final Lock lock;

    /**
     * Constructor for <code>Producer</code> with product buffer.
     *
     * @param productBuffer product buffer where products are extracted from
     * @param lock          lock for this producer
     */
    public Producer(Stack<String> productBuffer, Lock lock) throws IllegalArgumentException {
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        } else if (lock == null) {
            throw new IllegalArgumentException("Lock can't be null");
        }

        Thread thread = new Thread(this, "producer");
        this.name = "producer_" + thread.getId();
        this.productBuffer = productBuffer;
        this.lock = lock;

        thread.start();
    }

    /**
     * Put produced product into productBuffer on first place.
     */
    private void putProduct() throws IllegalArgumentException {
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        }

        producedProduct = name + ":product_" + numberProducedProduct;
        productBuffer.add(producedProduct);
        numberProducedProduct++;
    }

    @Override
    public void run() throws IllegalArgumentException {
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        } else if (lock == null) {
            throw new IllegalArgumentException("Lock can't be null");
        }

        try {
            while (true) {
                lock.lock();

                putProduct();
                System.out.println(name + " put: " + producedProduct);

                lock.unlock();
                Thread.sleep(1000);

                if (isProducerStopped) {
                    throw new InterruptedException();
                }
            }
        } catch (InterruptedException e) {
            System.out.println(name + " finished");
        }
    }

    /** Stop producer. */
    public void stop() {
        isProducerStopped = true;
    }

}
