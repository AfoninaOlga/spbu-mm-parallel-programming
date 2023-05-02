package test.java.ershov.consumer;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;

import main.java.ershov.consumer.Consumer;

import java.util.Stack;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

import static org.junit.jupiter.api.Assertions.*;

@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class ConsumerTest {

    @Test
    public void testPositive() {
        Stack<String> productBuffer = new Stack<>();
        Lock lock = new ReentrantLock();

        String product1 = "product1";
        productBuffer.add(product1);

        Consumer consumer = new Consumer(productBuffer, lock);

        consumer.stop();

        assertEquals(productBuffer, new Stack<>());
    }

	@Test
    public void testWithNullProductBuffer() {
        assertThrows(IllegalArgumentException.class, () -> {
            new Consumer(null, new ReentrantLock());
        });
    }

    @Test
    public void testWithNullLock() {
        assertThrows(IllegalArgumentException.class, () -> {
            new Consumer(new Stack<>(), null);
        });
    }

    @Test
    public void testWithUnCorrectLock() {
        assertThrows(IllegalArgumentException.class, () -> {
            new Consumer(new Stack<>(), new ReentrantReadWriteLock.ReadLock(new ReentrantReadWriteLock()) {
            });
        });
    }

}
