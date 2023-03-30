package main.java.consumer;

import java.util.Stack;
import java.util.concurrent.Semaphore;

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
    private String extractedProduct;
    /** Product buffer. */
    private final Stack<String> productBuffer;
    /** Mutex for this consumer. */
    private final Semaphore mutex;

    /**
     * Constructor for <code>Consumer</code> with product buffer and mutex.
     *
     * @param productBuffer product buffer where products are extracted from
     * @param mutex         mutex for this consumer
     */
    public Consumer(Stack<String> productBuffer, Semaphore mutex) {
        // TODO: проверить mutex, что он Semaphore(1)
        Thread thread = new Thread(this, "consumer");
        this.name = "consumer_" + thread.getId();
        this.productBuffer = productBuffer;
        this.mutex = mutex;

        thread.start();
    }

    /**
     * Extract last produced product from productBuffer and put into extractedProduct.
     */
    private void extractProduct() {
        this.extractedProduct = productBuffer.pop();
    }

    @Override
    public void run() {
        try {
            while (true) {
                mutex.acquire();

                if (!productBuffer.empty()) {
                    extractProduct();
                    System.out.println(name + " extracted: " + extractedProduct);
                }

                mutex.release();
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
