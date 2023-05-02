package main.java.ershov.threadpool;

import main.java.ershov.mytask.IMyTask;

import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;

public abstract class MyAbstractThread extends Thread {

    protected final ThreadPool threadPool;
    private final Queue<Object> tasks = new LinkedBlockingQueue<>();
    protected volatile CancellationTokenSource cancellationTokenSource;

    public MyAbstractThread(ThreadPool threadPool, String name, CancellationTokenSource cancellationTokenSource) {
        super(threadPool, name);
        this.threadPool = threadPool;
        this.cancellationTokenSource = cancellationTokenSource;
    }

    public <TResult> void setTask(IMyTask<TResult> task) {
        if (!this.isInterrupted()) {
            tasks.add(task);
        }

        synchronized (this) {
            this.notify();
        }
    }

    public Queue<Object> getTasks() {
        return tasks;
    }

    @Override
    public void run() {
        try {
            while(true) {
                executeTask(tasks);

                if (cancellationTokenSource.getCancellationToken()) {
                    this.interrupt();
                }
            }
        } catch (InterruptedException e) {
            System.out.println(this.getName() + " is finished");
        }
    }

    protected abstract void executeTask(Queue<Object> tasks) throws InterruptedException;

}
