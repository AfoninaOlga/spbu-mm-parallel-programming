package ershov.p2pchat;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;

public class P2PChatSocket extends Thread {

	/** Target socket. */
	private final Socket socket;
	/** Socket writer. */
	private final PrintWriter out;
	/** P2P chat. */
	private final P2PChat p2PChat;
	private volatile CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
	private volatile boolean isStopped = false;

	public P2PChatSocket(Socket socket, P2PChat p2PChat, CancellationTokenSource cancellationTokenSource)
			throws IOException {
		this.socket = socket;
		this.out = new PrintWriter(socket.getOutputStream(), true);
		this.p2PChat = p2PChat;
		this.cancellationTokenSource = cancellationTokenSource;

		start();
	}

	public InetAddress getSocketInetAddress() {
		return socket.getInetAddress();
	}

	@Override
	public void run() {
		String message = "";
		try (BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()))) {
			while (true) {
				//message = in.readLine();

				if (message != null && message.contains("User:")) {
					p2PChat.connectToSocket(message.split(":")[1]);
				} else if (message != null && message.equals("Stop")) {
					this.interrupt();
				}

				if (message != null && !message.equals("")) {
					p2PChat.getMessages().add("Message from " + socket.getInetAddress() + ": " + message);
				}

				if (cancellationTokenSource.getCancellationToken() || isStopped) {
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
			System.out.println("P2P Chat Socket with " + socket.getInetAddress() + " stopped");
			try {
				out.close();
				socket.close();
			} catch (IOException e) {
				System.out.println(e.getMessage());
			}
		}
	}

	public void send(String message) {
		if (this.isInterrupted()) {
			return;
		}

		out.println(message);

		if (message.equals("Stop")) {
			isStopped = true;
		}
	}

}
