package main.java.ershov;

import main.java.ershov.consumer.Consumer;
import main.java.ershov.producer.Producer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.*;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

/**
 * Main class for producer-consumer task.
 *
 * @author yersh2000@gmail.com
 */
public class Main {

    public static void main(String[] args) throws InterruptedException, IOException, IllegalArgumentException {
        if (args == null || args.length < 2 || args[0] == null || args[1] == null) {
            throw new IllegalArgumentException("Number of producers and consumers entered incorrectly");
        }

        int numOfProducers;
        int numOfConsumers;
        try {
            numOfProducers = Integer.parseInt(args[0]);
            numOfConsumers = Integer.parseInt(args[1]);
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("Number of producers and consumers entered incorrectly");
        }

        boolean isStopped;
        if (args.length < 3) {
            isStopped = false;
        } else {
            isStopped = Boolean.parseBoolean(args[2]);
        }

        Stack<String> productBuffer = new Stack<>();
        // Lock for producers and consumers.
        final Lock lock = new ReentrantLock();

        List<Producer> producers = new ArrayList<>();
        List<Consumer> consumers = new ArrayList<>();

        for (int i = 0; i < numOfProducers; i++) {
            producers.add(new Producer(productBuffer, lock));
        }

        for (int i = 0; i < numOfConsumers; i++) {
            consumers.add(new Consumer(productBuffer, lock));
        }

        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        while(!isStopped && reader.readLine() == null) {
            System.out.println("Step");
            Thread.sleep(1000);
        }
        reader.close();

        for (Producer producer: producers) {
            producer.stop();
        }

        for (Consumer consumer: consumers) {
            consumer.stop();
        }
    }

}
