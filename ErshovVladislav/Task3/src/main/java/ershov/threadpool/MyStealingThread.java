package main.java.ershov.threadpool;

import main.java.ershov.mytask.IMyTask;

import java.util.Queue;

public class MyStealingThread extends MyAbstractThread {

    public MyStealingThread(ThreadPool threadPool, String name, CancellationTokenSource cancellationTokenSource) {
        super(threadPool, name, cancellationTokenSource);
    }

    @Override
    public void executeTask(Queue<Object> tasks) throws InterruptedException {
        if (tasks.isEmpty() && !cancellationTokenSource.getCancellationToken()) {
            IMyTask<?> stolenTask = stealTask();
            if (stolenTask != null) {
                tasks.add(stolenTask);
            }
        }

        if (tasks.isEmpty()) {
            Thread.sleep(1000);
        } else {
            IMyTask<?> task = (IMyTask<?>) tasks.poll();
            task.run();
            System.out.println(this.getName() + " executed task");
        }
    }

    private IMyTask<?> stealTask() {
        for (MyAbstractThread thread: threadPool.getThreads()) {
            if (!thread.getTasks().isEmpty()) {
                return (IMyTask<?>) thread.getTasks().poll();
            }
        }

        return null;
    }
}
