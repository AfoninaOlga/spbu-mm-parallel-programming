package ershov;

import java.io.IOException;

import javafx.application.Application;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.ScrollPane;
import javafx.scene.control.TextField;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.HBox;
import javafx.scene.paint.Paint;
import javafx.scene.text.Text;
import javafx.stage.Stage;

import ershov.p2pchat.P2PChat;

public class Main extends Application {

	private final int PORT = 8093;
	private P2PChat chat;
	private String ip = "";
	private String message = "";

	Button connectToButton;
	Button sendButton;
	Button refreshButton;
	Text chatAreaText;
	Text errorMessageText;

	public static void main(String[] args) {
		launch(args);
	}

	private void createAreaText(GridPane grid) {
		chatAreaText = new Text();
		chatAreaText.setText("Chat started");

		ScrollPane scroll = new ScrollPane();
		scroll.autosize();
		scroll.setStyle("-fx-border-color: green;");
		scroll.setContent(chatAreaText);

		grid.add(scroll, 0, 1, 3, 10);
	}

	private void createErrorMessage(GridPane grid) {
		errorMessageText = new Text();
		errorMessageText.setText("");
		errorMessageText.setUnderline(true);
		errorMessageText.setFill(Paint.valueOf("red"));

		grid.add(errorMessageText, 0, 13);
	}

	private void createSendSection(GridPane grid) {
		Label sendLabel = new Label("Enter message:");
		grid.add(sendLabel, 0, 11);

		TextField sendTextField = new TextField();
		sendTextField.textProperty().addListener((observable, oldValue, newValue) -> {
			message = newValue;
		});
		grid.add(sendTextField, 1, 11);

		sendButton = new Button();
		sendButton.setText("Send");
		sendButton.setOnAction(new EventHandler<ActionEvent>() {

			@Override
			public void handle(ActionEvent event) {
				chat.send(message);
				sendTextField.setText("");
			}

		});
		grid.add(sendButton, 2, 11);

		refreshButton = new Button();
		refreshButton.setText("Refresh chat");
		refreshButton.setOnAction(new EventHandler<ActionEvent>() {

			@Override
			public void handle(ActionEvent event) {
				String text = "";
				for (String message : chat.getMessages()) {
					text += message + "\n";
				}
				chatAreaText.setText(text);
			}

		});
		grid.add(refreshButton, 2, 12);
	}

	private void createConnectSection(GridPane grid) {
		Label connectToLabel = new Label("Enter ip:");
		grid.add(connectToLabel, 0, 0);

		TextField connectToTextField = new TextField();
		connectToTextField.textProperty().addListener((observable, oldValue, newValue) -> {
			ip = newValue;
		});
		grid.add(connectToTextField, 1, 0);

		connectToButton = new Button();
		connectToButton.setText("Connect");
		connectToButton.setOnAction(new EventHandler<ActionEvent>() {

			@Override
			public void handle(ActionEvent event) {
				try {
					chat.connectToSocket(ip);
					errorMessageText.setText("");
				} catch (IOException e) {
					errorMessageText.setText(e.getMessage());
					System.out.println(e.getMessage());
				}
			}

		});
		grid.add(connectToButton, 2, 0);
	}

	@Override
	public void init() throws Exception {
		super.init();
		System.out.println("P2P Chat Application init");
	}

	@Override
	public void start(Stage primaryStage) throws Exception {
		chat = new P2PChat(PORT);

		primaryStage.setTitle("P2P Chat Application start");

		GridPane grid = new GridPane();
		grid.setAlignment(Pos.TOP_CENTER);
		grid.setHgap(10);
		grid.setVgap(10);
		grid.setPadding(new Insets(25, 25, 25, 25));

		createConnectSection(grid);
		createAreaText(grid);
		createSendSection(grid);
		createErrorMessage(grid);

		Scene scene = new Scene(grid, 800, 600);
		primaryStage.setScene(scene);

		primaryStage.show();
	}

	@Override
	public void stop() throws Exception {
		chat.close();
		super.stop();
		System.out.println("P2P Chat Application stop");
	}

}
