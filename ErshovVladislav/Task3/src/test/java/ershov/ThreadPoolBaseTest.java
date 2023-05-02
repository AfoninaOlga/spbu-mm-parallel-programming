package test.java.ershov;

import main.java.ershov.mytask.IMyTask;
import main.java.ershov.mytask.MyTask;
import main.java.ershov.threadpool.ThreadPool;
import main.java.ershov.threadpool.WorkStrategy;
import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import java.util.concurrent.ConcurrentSkipListSet;
import java.util.function.Function;

import static org.junit.jupiter.api.Assertions.*;

public class ThreadPoolBaseTest {

    @Test
    public void testNumberOfThreadSharing() throws InterruptedException {
        ThreadPool threadPool = new ThreadPool(8, WorkStrategy.WORK_SHARING);
        Set<Long> threadIds = new ConcurrentSkipListSet<>();

        for (int i = 0; i < 100; i++) {
            Function<Void, String> function = new Function<>() {
                @Override
                public String apply(Void unused) {
                    threadIds.add(Thread.currentThread().getId());
                    return "";
                }
            };

            IMyTask<String> task = new MyTask<>(function, threadPool);
            threadPool.enqueue(task);
        }

        threadPool.run();
        Thread.sleep(2000);
        threadPool.close();

        assertTrue(threadIds.size() == 8);
    }

    @Test
    public void testNumberOfThreadStealing() throws InterruptedException {
        ThreadPool threadPool = new ThreadPool(30, WorkStrategy.WORK_STEALING);
        Set<Long> threadIds = new ConcurrentSkipListSet<>();

        for (int i = 0; i < 300; i++) {
            Function<Void, String> function = new Function<>() {
                @Override
                public String apply(Void unused) {
                    threadIds.add(Thread.currentThread().getId());
                    return "";
                }
            };

            IMyTask<String> task = new MyTask<>(function, threadPool);
            threadPool.enqueue(task);
        }

        threadPool.run();
        Thread.sleep(2000);
        threadPool.close();

        assertTrue(threadIds.size() == 30);
    }

    @Test
    public void testWithOneTask() throws InterruptedException {
        ThreadPool threadPool = new ThreadPool(4, WorkStrategy.WORK_SHARING);

        IMyTask<String> task = new MyTask<>(s -> "$", threadPool);
        threadPool.enqueue(task);

        threadPool.run();
        Thread.sleep(1000);
        threadPool.close();

        assertTrue(task.isCompleted() || task.getFunctionErrorMessage() != null);
    }

    @Test
    public void testWithTaskMoreThenThread() throws InterruptedException {
        ThreadPool threadPool = new ThreadPool(4, WorkStrategy.WORK_STEALING);

        List<IMyTask<?>> tasks = new ArrayList<>();
        for (int i = 0; i < 10; i++) {
            IMyTask<String> task = new MyTask<>(s -> "$", threadPool);
            tasks.add(task);

            IMyTask<Integer> task2 = new MyTask<>(s -> 2, threadPool);
            tasks.add(task2);
        }

        for (IMyTask<?> task: tasks) {
            threadPool.enqueue(task);
        }

        threadPool.run();
        Thread.sleep(2000);
        threadPool.close();

        for (IMyTask<?> task: tasks) {
            assertTrue(task.isCompleted() || task.getFunctionErrorMessage() != null);
        }
    }

    @Test
    public void testWithContinueWithTask() throws InterruptedException {
        ThreadPool threadPool = new ThreadPool(4, WorkStrategy.WORK_SHARING);

        IMyTask<String> task0 = new MyTask<>(s -> "$", threadPool);
        IMyTask<String> task1 = task0.continueWith(s -> s + "#");
        IMyTask<String> task2 = task0.continueWith(s -> s + "#");
        IMyTask<String> task3 = task1.continueWith(s -> s + "%");

        threadPool.enqueue(task0);
        threadPool.enqueue(task1);
        threadPool.enqueue(task2);
        threadPool.enqueue(task3);

        threadPool.run();
        Thread.sleep(1000);
        threadPool.close();

        assertTrue(task0.isCompleted() || task0.getFunctionErrorMessage() != null);
        assertTrue(task1.isCompleted() || task1.getFunctionErrorMessage() != null);
        assertTrue(task2.isCompleted() || task2.getFunctionErrorMessage() != null);
        assertTrue(task3.isCompleted() || task3.getFunctionErrorMessage() != null);
    }

}
