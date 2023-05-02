package main.java.ershov.threadpool;

import main.java.ershov.mytask.IMyTask;

import java.util.Queue;

public class MySharingThread extends MyAbstractThread {

    public MySharingThread(ThreadPool threadPool, String name, CancellationTokenSource cancellationTokenSource) {
        super(threadPool, name, cancellationTokenSource);
    }

    @Override
    protected void executeTask(Queue<Object> tasks) throws InterruptedException {
        if (tasks.size() > 1 && !cancellationTokenSource.getCancellationToken()) {
            shareTask(tasks);
        }

        if (tasks.isEmpty()) {
            synchronized (this) {
                this.wait(1000);
            }
        }

        IMyTask<?> task = (IMyTask<?>) tasks.poll();
        if (task != null) {
            task.run();
            System.out.println(this.getName() + " executed task");
        }
    }

    private void shareTask(Queue<Object> tasks) {
        for (MyAbstractThread thread: threadPool.getThreads()) {
            if (thread.getTasks().size() < tasks.size()) {
                thread.getTasks().add(tasks.poll());

                synchronized (thread) {
                    thread.notify();
                }

                return;
            }
        }
    }

}
