package main.java;

import main.java.consumer.Consumer;
import main.java.producer.Producer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.*;
import java.util.concurrent.Semaphore;

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

        Stack<String> productBuffer = new Stack<>();
        // A special case of a semaphore is a mutex.
        Semaphore mutex = new Semaphore(1);

        List<Producer> producers = new ArrayList<>();
        List<Consumer> consumers = new ArrayList<>();

        for (int i = 0; i < numOfProducers; i++) {
            producers.add(new Producer(productBuffer, mutex));
        }

        for (int i = 0; i < numOfConsumers; i++) {
            consumers.add(new Consumer(productBuffer, mutex));
        }

        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        while(reader.readLine() == null) {
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
