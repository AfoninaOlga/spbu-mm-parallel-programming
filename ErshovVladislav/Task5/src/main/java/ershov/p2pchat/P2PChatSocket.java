package ershov.p2pchat;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.Socket;

public class P2PChatSocket extends Thread {

	/** This thread. */
    private final Thread thread;
	/** Target socket. */
	private final Socket socket;
	/** Socket read stream. */
    private final BufferedReader in;
    /** Socket write stream. */
    private final BufferedWriter out;
    /** P2P chat. */
    private final P2PChat p2PChat;
    private volatile CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

	public P2PChatSocket(Socket socket, P2PChat p2PChat, CancellationTokenSource cancellationTokenSource) throws IOException {
		this.socket = socket;
		this.in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
        this.out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
        this.p2PChat = p2PChat;
        this.cancellationTokenSource = cancellationTokenSource;
        this.thread = new Thread(this);

    	thread.start();
	}

    public InetAddress getSocketInetAddress() {
    	return socket.getInetAddress();
    }

    @Override
    public void run() {
        String message = "";
        try {
            while (true) {
    			message = in.readLine();

                if (message == null || message.equals("")) {
                	continue;
                } else if (message.contains("Socket:")) {
                	p2PChat.connectToSocket(message.split(":")[1]);
                } else if (message.equals("Stop")) {
                	this.interrupt();
                }

                p2PChat.getMessages().add("Message from " + socket.getInetAddress() + ": " + message);

                if (cancellationTokenSource.getCancellationToken()) {
                    this.interrupt();
                }

                if (this.isInterrupted()) {
            		break;
            	}
            }
        } catch (Exception e) {
			System.out.println(e.getMessage());
        } finally {
        	p2PChat.getMessages().add("User " + socket.getInetAddress() + " diconected");
        	System.out.println("P2P Chat Socket stop");
        	try {
	        	in.close();
				out.close();
				socket.close();
			} catch (IOException e) {
				System.out.println(e.getMessage());
			}
        }
    }

    public void send(String message) {
    	if (thread.isInterrupted()) {
    		return;
    	}

        try {
            out.write(message + "\n");
            out.flush();
        } catch (IOException e) {
        	System.out.println(e.getMessage());
        }
    }

}
