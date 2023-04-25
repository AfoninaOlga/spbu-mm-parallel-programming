package main.java.ershov.threadpool;

public class CancellationTokenSource {

    private volatile boolean cancellationToken = false;

    public boolean getCancellationToken() {
        return cancellationToken;
    }

    public void setCancelToken() {
        cancellationToken = true;
    }

}
