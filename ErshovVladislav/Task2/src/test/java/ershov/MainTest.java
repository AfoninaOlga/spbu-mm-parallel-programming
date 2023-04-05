package ershov;

import org.junit.jupiter.api.Test;

import java.io.IOException;
import java.util.Stack;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;

import main.java.ershov.Main;

public class MainTest {

    @Test
    public void testWithNullArgs() {
        assertThrows(IllegalArgumentException.class, () -> {
            Main.main(null);
        });
    }

    @Test
    public void testWithNullArgsValue() {
        String[] args = new String[] { null, null };
        assertThrows(IllegalArgumentException.class, () -> {
            Main.main(args);
        });
    }

    @Test
    public void testWithNotIntegerArgsValue() {
        String[] args = new String[] { "test", "test" };
        assertThrows(IllegalArgumentException.class, () -> {
            Main.main(args);
        });
    }

}
