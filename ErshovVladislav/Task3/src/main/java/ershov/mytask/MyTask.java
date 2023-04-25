package main.java.ershov.mytask;

import main.java.ershov.threadpool.ThreadPool;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;

public class MyTask<TResult> implements IMyTask<TResult> {

    private final Function<Void, TResult> function;
    private final ThreadPool threadPool;
    private volatile TResult resultTask = null;
    private volatile boolean isCompletedTask = false;
    private volatile String functionErrorMessage = null;
    private final List<Object> continuations = new ArrayList<>();

    public MyTask(Function<Void, TResult> function, ThreadPool threadPool) {
        this.function = function;
        this.threadPool = threadPool;
    }

    @Override
    public <TNewResult> IMyTask<TNewResult> continueWith(Function<TResult, TNewResult> function) {
        if (isCompleted() || getFunctionErrorMessage() != null) {
            throw new IllegalStateException("The original task has already been submitted for execution");
        }

        Function<Void, TResult> getResultFunction = input -> resultTask;
        IMyTask<TNewResult> newTask = new MyTask<>(getResultFunction.andThen(function), threadPool);

        if (isCompleted()) {
            threadPool.enqueue(newTask);
        } else {
            continuations.add(newTask);
        }

        return newTask;
    }

    @Override
    public boolean isCompleted() {
        return isCompletedTask;
    }

    @Override
    public String getFunctionErrorMessage() {
        return functionErrorMessage;
    }

    @Override
    public TResult result() throws InterruptedException, AggregateException {
        while (!isCompleted() || getFunctionErrorMessage() != null) {
            if (getFunctionErrorMessage() != null) {
                throw new AggregateException(getFunctionErrorMessage());
            }

            Thread.sleep(1000);
        }

        return resultTask;
    }

    @Override
    public void run() {
        if (getFunctionErrorMessage() != null || isCompleted()) {
            return;
        }

        try {
            resultTask = function.apply(null);
            isCompletedTask = true;
        } catch (Exception exception) {
            functionErrorMessage = exception.getMessage();

            for (Object task: continuations) {
                ((IMyTask<?>) task).setFunctionErrorMessage(exception.getMessage());
            }
        } finally {
            for (Object task: continuations) {
                threadPool.enqueue((IMyTask<?>) task);
            }
            System.out.println("Task finished with " + resultTask);
        }
    }

    @Override
    public void setFunctionErrorMessage(String message) {
        functionErrorMessage = message;
    }

}
