package ershov.p2pchat;

import java.net.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.io.*;

public class P2PChat extends Thread implements AutoCloseable {

	/** Port for server and client sockets. */
	private final int port;
	/** List for p2p chat users ips. */
	private final Set<InetAddress> p2PChatUserIps = Collections.synchronizedSet(new HashSet<>());
	/** List for all messages. */
	private final List<String> messages = Collections.synchronizedList(new ArrayList<>());
	private volatile CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

	public P2PChat(int port) {
		this.port = port;
		start();
	}

	@Override
	public void close() throws Exception {
		sendToAll("Stop");
		cancellationTokenSource.setCancelToken();
	}

	public void connect(String newP2PChatUserIp) throws UnknownHostException {
		connect(InetAddress.getByName(newP2PChatUserIp));
	}

	private void connect(InetAddress newP2PChatUserIp) {
		if (newP2PChatUserIp != null && !p2PChatUserIps.contains(newP2PChatUserIp)) {
			sendUserIpsToNewUserIp(newP2PChatUserIp);
			p2PChatUserIps.add(newP2PChatUserIp);
			messages.add("Connect to " + newP2PChatUserIp);
		}
	}

	public List<String> getMessages() {
		return messages;
	}

	private void recieve(Socket socket) {
		String message = "";
		try (BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()))) {
			message = in.readLine();
		} catch (IOException e) {
			System.out.println(e.getMessage());
		}

		if (message != null && message.contains("UserIps:")) {
			for (String p2PChatUserIp : message.split(":")) {
				if (p2PChatUserIp.equals("UserIps")) {
					continue;
				}

				try {
					connect(p2PChatUserIp.replace("/", ""));
				} catch (UnknownHostException e) {
					messages.add("Not connect to " + p2PChatUserIp);
					System.out.println(e.getMessage());
				}
			}

			connect(socket.getInetAddress());
		} else if (message != null && message.equals("Stop")) {
			p2PChatUserIps.remove(socket.getInetAddress());
		} else if (message != null) {
			connect(socket.getInetAddress());
		}

		if (message != null && !message.isEmpty()) {
			messages.add("From " + socket.getInetAddress() + ": " + message);
		}
	}

	@Override
	public void run() {
		try (ServerSocket server = new ServerSocket(port)) {
			server.setSoTimeout(2000);
			while (true) {
				try (Socket socket = server.accept()) {
					socket.getInetAddress();
					recieve(socket);
				} catch (IOException e) {
					System.out.println(e.getMessage());
				}

				if (cancellationTokenSource.getCancellationToken()) {
					this.interrupt();
				}

				if (this.isInterrupted()) {
					break;
				}
			}
		} catch (IOException e) {
			System.out.println(e.getMessage());
		} finally {
			System.out.println("P2P Chat stop");
			p2PChatUserIps.clear();
			messages.clear();
		}
	}

	private void send(InetAddress p2PChatUserIp, String message) {
		try (Socket socket = new Socket(p2PChatUserIp, port);
				PrintWriter out = new PrintWriter(socket.getOutputStream(), true)) {
			out.println(message);
		} catch (IOException e) {
			messages.add("\"" + message + "\" not send to " + p2PChatUserIp);
			System.out.println(e.getMessage());
		}
	}

	public void sendToAll(String message) {
		messages.add("My message: " + message);
		for (InetAddress p2PChatUserIp : p2PChatUserIps) {
			send(p2PChatUserIp, message);
		}

		if (message.equals("Stop")) {
			cancellationTokenSource.setCancelToken();
		}
	}

	private void sendUserIpsToNewUserIp(InetAddress newP2PChatUserIp) {
		String message = "UserIps:";
		for (InetAddress p2PChatUserIp : p2PChatUserIps) {
			message += p2PChatUserIp + ":";
		}
		send(newP2PChatUserIp, message);
	}

}
