package ershov.p2pchat;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.Socket;

public class P2PChatSocket extends Thread implements AutoCloseable {

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

	public P2PChatSocket(Socket socket, P2PChat p2PChat) throws IOException {
		this.socket = socket;
		this.in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
        this.out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
        this.p2PChat = p2PChat;
        this.thread = new Thread(this);
        System.out.println("P2PChatSocket:start");

    	thread.start();
	}

	@Override
	public void close() throws Exception {
    	thread.interrupt();
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
                    close();
                }

                p2PChat.getMessages().add(message);

                if (this.isInterrupted()) {
                	System.out.println("P2PChatSocket:break");
            		break;
            	}
            }
        } catch (Exception e) {
        	System.out.println("P2PChatSocket:5" + e.getMessage());
        	System.out.println(e.getMessage());
        } finally {
        	try {
        		System.out.println("P2PChatSocket:6");
	        	in.close();
				out.close();
				socket.close();
				System.out.println("P2PChatSocket:6a");
			} catch (IOException e) {
				System.out.println("P2PChatSocket:7");
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
            System.out.println("P2PChatSocket:10");
        } catch (IOException e) {
        	System.out.println("P2PChatSocket:11");
        	System.out.println(e.getMessage());
        }
    }

}
