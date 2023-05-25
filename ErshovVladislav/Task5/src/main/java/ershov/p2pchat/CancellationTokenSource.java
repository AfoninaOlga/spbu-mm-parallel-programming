package ershov.p2pchat;

public class CancellationTokenSource {

	private volatile boolean cancellationToken = false;

    public boolean getCancellationToken() {
        return cancellationToken;
    }

    public void setCancelToken() {
        cancellationToken = true;
    }

}
