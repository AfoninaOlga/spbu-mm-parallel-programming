package main.java.ershov.mytask;

import java.util.function.Function;

public interface IMyTask<TResult> extends Runnable {

    <TNewResult> IMyTask<TNewResult> continueWith(Function<TResult, TNewResult> fun);

    boolean isCompleted();

    String getFunctionErrorMessage();

    TResult result() throws InterruptedException, AggregateException;

    void setFunctionErrorMessage(String message);

}
