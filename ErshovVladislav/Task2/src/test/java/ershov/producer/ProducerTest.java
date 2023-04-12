package ershov.producer;

import main.java.ershov.producer.Producer;
import org.junit.jupiter.api.Test;

import java.util.Stack;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

import static org.junit.jupiter.api.Assertions.*;

public class ProducerTest {

    @Test
    public void testPositive() throws InterruptedException {
        Stack<String> productBuffer = new Stack<>();
        Lock lock = new ReentrantLock();

        Producer producer = new Producer(productBuffer, lock);

        producer.stop();
        Thread.sleep(1000);

        assertFalse(productBuffer.empty());
    }

    @Test
    public void testWithNullProductBuffer() {
        assertThrows(IllegalArgumentException.class, () -> {
            new Producer(null, new ReentrantLock());
        });
    }

    @Test
    public void testWithNullLock() {
        assertThrows(IllegalArgumentException.class, () -> {
            new Producer(new Stack<>(), null);
        });
    }

    @Test
    public void testWithUnCorrectLock() {
        assertThrows(IllegalArgumentException.class, () -> {
            new Producer(new Stack<>(), new ReentrantReadWriteLock.ReadLock(new ReentrantReadWriteLock()) {
            });
        });
    }

}
