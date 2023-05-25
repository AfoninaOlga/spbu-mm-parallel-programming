package main.java.ershov;

import main.java.ershov.mytask.IMyTask;
import main.java.ershov.mytask.MyTask;
import main.java.ershov.threadpool.ThreadPool;
import main.java.ershov.threadpool.WorkStrategy;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

/**
 * Main class for ThreadPool.
 *
 * @author yersh2000@gmail.com
 */
public class Main {

    public static void main(String[] args) throws InterruptedException, IOException, IllegalArgumentException {
        if (args == null || args.length < 2 || args[0] == null || args[1] == null) {
            throw new IllegalArgumentException("Number of threads entered incorrectly");
        }

        int numOfThreads;
        try {
            numOfThreads = Integer.parseInt(args[0]);
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("Number of threads entered incorrectly");
        }

        WorkStrategy workStrategy;
        if (args[1].equals("workStealing")) {
            workStrategy = WorkStrategy.WORK_STEALING;
        } else {
            workStrategy = WorkStrategy.WORK_SHARING;
        }

        ThreadPool threadPool = new ThreadPool(numOfThreads, workStrategy);

        List<IMyTask<?>> tasks = new ArrayList<>();
        for (int i = 0; i < 2; i++) {
            IMyTask<String> task = new MyTask<>(s -> "$", threadPool);
            task.continueWith(s -> s + "#");
            task.continueWith(s -> s + "&");

            tasks.add(task);

            IMyTask<Integer> task2 = new MyTask<>(s -> 2, threadPool);
            task2.continueWith(s -> s + 3);
            tasks.add(task2);
        }

        for (IMyTask<?> task: tasks) {
            threadPool.enqueue(task);
        }
        threadPool.run();

        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        while(reader.readLine() == null) {
            System.out.println("Step");
            Thread.sleep(1000);
        }
        reader.close();

        threadPool.close();
    }

}
