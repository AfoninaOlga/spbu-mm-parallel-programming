package main.java.producer;

import java.util.Stack;
import java.util.concurrent.Semaphore;

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
    /** Mutex for this manufacturer. */
    private final Semaphore mutex;

    /**
     * Constructor for <code>Producer</code> with product buffer.
     *
     * @param productBuffer product buffer where products are extracted from
     * @param mutex         mutex for this manufacturer
     */
    public Producer(Stack<String> productBuffer, Semaphore mutex) throws IllegalArgumentException {
        if (productBuffer == null) {
            throw new IllegalArgumentException("Product buffer can't be null");
        } else if (mutex == null) {
            throw new IllegalArgumentException("Mutex can't be null");
        }

        // TODO: проверить mutex, что он Semaphore(1)
        Thread thread = new Thread(this, "producer");
        this.name = "producer_" + thread.getId();
        this.productBuffer = productBuffer;
        this.mutex = mutex;

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
        } else if (mutex == null) {
            throw new IllegalArgumentException("Mutex can't be null");
        }

        try {
            while (true) {
                mutex.acquire();

                putProduct();
                System.out.println(name + " put: " + producedProduct);

                mutex.release();
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
