package ershov;

import java.io.IOException;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ershov.p2pchat.P2PChat;

@RestController
public class P2PChatController {

	private P2PChat p2PChat = null;

	@RequestMapping("/startWithoutConnect")
	public void startWithoutConnect(@RequestParam(value = "port") int port) {
		if (p2PChat == null) {
			p2PChat = new P2PChat(port);
		}
	}

	@RequestMapping("/startWithConnect")
	public void startWithConnect(@RequestParam(value = "ip") String ip, @RequestParam(value = "port") int port) {
		if (p2PChat == null) {
			try {
				p2PChat = new P2PChat(ip, port);
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

	@RequestMapping("/getMessagesCount")
	public void getMessagesCount() {
		if (p2PChat != null) {
			p2PChat.getMessages().size();
		}
	}

}
