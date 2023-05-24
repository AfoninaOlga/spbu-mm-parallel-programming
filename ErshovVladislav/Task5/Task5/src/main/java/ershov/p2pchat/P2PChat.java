package ershov.p2pchat;

import java.net.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.io.*;

public class P2PChat extends Thread implements AutoCloseable {

	/** Port for server and client sockets. */
	private final int port;
	/** This thread. */
    private final Thread thread;
	/** List for p2p chat sockets. */
    private final List<P2PChatSocket> p2PChatSockets = Collections.synchronizedList(new ArrayList<>());
    /** List for all messages. */
    private final List<String> messages = Collections.synchronizedList(new ArrayList<>());

    public P2PChat(int port) {
    	this.port = port;
    	this.thread = new Thread(this);

    	System.out.println("P2PChat:startWithoutConnect");
    	thread.start();
    }

    public P2PChat(String ip, int port) throws UnknownHostException, IOException {
    	this.port = port;
    	Socket socket = new Socket(ip, port);
    	p2PChatSockets.add(new P2PChatSocket(socket, this));
    	this.thread = new Thread(this);

    	thread.start();
    }

    @Override
	public void close() throws Exception {
    	for (P2PChatSocket p2PChatSocket: p2PChatSockets) {
    		p2PChatSocket.send("Stop");
    		p2PChatSocket.close();
    	}

    	thread.interrupt();
	}

    public void connectToSocket(String ip) throws UnknownHostException, IOException {
    	Socket socket = new Socket(ip, port);
    	p2PChatSockets.add(new P2PChatSocket(socket, this));
    }

    public List<String> getMessages() {
    	return messages;
    }

    @Override
    public void run() {
		try (ServerSocket server = new ServerSocket(port)) {
			server.setSoTimeout(10000);
            while (true) {
            	try (Socket socket = server.accept()) {
            		P2PChatSocket p2PChatSocket = new P2PChatSocket(socket, this);
            		sendSocketsToNewSocket(p2PChatSocket);
            		p2PChatSockets.add(p2PChatSocket);
	            } catch (IOException e) {
	            	System.out.println("P2PChat small:" + e.getMessage());
	    			System.out.println(e.getMessage());
	    		}

            	if (this.isInterrupted()) {
            		break;
            	}
            }
		} catch (IOException e) {
			System.out.println("P2PChat big:" + e.getMessage());
			System.out.println(e.getMessage());
		} finally {
			p2PChatSockets.clear();
            messages.clear();
        }
    }

    public void send(String message) {
    	System.out.println("P2PChat:1:" + message);
    	messages.add(message);
    	for (P2PChatSocket p2PChatSocket: p2PChatSockets) {
    		System.out.println("P2PChat:2");
    		p2PChatSocket.send(message);
    	}
    }

    private void sendSocketsToNewSocket(P2PChatSocket newSocket) {
    	for (P2PChatSocket p2PChatSocket: p2PChatSockets) {
    		newSocket.send("Socket:" + p2PChatSocket.getSocketInetAddress());
    	}
    }

}
