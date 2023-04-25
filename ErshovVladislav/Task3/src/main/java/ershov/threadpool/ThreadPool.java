package main.java.ershov.threadpool;

import main.java.ershov.mytask.IMyTask;

import java.util.ArrayList;
import java.util.List;

public class ThreadPool implements Runnable, AutoCloseable {

    private final int numOfThreads;
    private final List<MyAbstractThread> threads = new ArrayList<>();
    private volatile CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public ThreadPool(int numOfThreads, WorkStrategy workStrategy) {
        this.numOfThreads = numOfThreads;

        for (int i = 0; i < numOfThreads; i++) {
            MyAbstractThread thread;
            if (workStrategy.equals(WorkStrategy.WORK_SHARING)) {
                thread = new MySharingThread(this, "myThread " + i, cancellationTokenSource);
            } else {
                thread = new MyStealingThread(this, "myThread " + i, cancellationTokenSource);
            }

            threads.add(thread);
        }
    }

    @Override
    public void close() {
        cancellationTokenSource.setCancelToken();
    }

    @Override
    public void run() {
        for (MyAbstractThread thread: threads) {
            thread.start();
        }
    }

    public <TResult> void enqueue(IMyTask<TResult> task) {
        for (MyAbstractThread thread: threads) {
            if (thread.getTasks().isEmpty()) {
                thread.setTask(task);
                return;
            }
        }

        int randomIndex = (int) (Math.random() * (numOfThreads - 1));
        threads.get(randomIndex).setTask(task);
    }

    public List<MyAbstractThread> getThreads() {
        return threads;
    }

}
