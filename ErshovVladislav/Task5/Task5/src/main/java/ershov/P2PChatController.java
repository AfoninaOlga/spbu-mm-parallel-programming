package ershov;

import java.io.IOException;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ershov.p2pchat.P2PChat;

@RestController
public class P2PChatController {

	private final int PORT = 8092;
	private P2PChat p2PChat = null;

	@RequestMapping("/startWithoutConnect")
	public void startWithoutConnect() {
		if (p2PChat == null) {
			p2PChat = new P2PChat(PORT);
		}
	}

	@RequestMapping("/startWithConnect")
	public void startWithConnect(@RequestParam(value = "ip") String ip) {
		if (p2PChat == null) {
			try {
				p2PChat = new P2PChat(ip, PORT);
			} catch (IOException e) {
				System.out.println(e.getMessage());
			}
		}
	}

	@RequestMapping("/send")
	public void send(@RequestParam(value = "message") String message) {
		if (p2PChat != null) {
			p2PChat.send(message);
		}
	}

	@RequestMapping("/getMessages")
	public void getMessages() {
		if (p2PChat != null) {
			p2PChat.getMessages();
		}
	}

}
