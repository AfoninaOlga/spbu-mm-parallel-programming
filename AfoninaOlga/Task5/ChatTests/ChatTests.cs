using System.Net.Sockets;
using Chat;
using Chat.NetworkMessage;
using static System.Net.IPEndPoint;

namespace ChatTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestDispose()
    {
        var connection = Connection.NewConnection(Parse("127.0.0.1:2000"));
        var chat = new Chat.Chat(connection, "user", new List<string>(), new List<MemberInfo>());
        chat.Dispose();
    }

    [Test]
    public void ConnectionTest()
    {
        var socketEndpoint = Parse("127.0.0.1:5000");
        var s = new Socket(socketEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        s.Bind(socketEndpoint);
        Message? message = null;

        var listenerThread = new Thread(() =>
        {
            s.Listen();
            var socket = s.Accept();
            var reader = new MessageReader(socket);
            message = reader.ReadMessage();
            socket.Close();
        });
        listenerThread.Start();
        Thread.Sleep(100);
        var connection = Connection.ConnectTo(socketEndpoint);
        listenerThread.Join();
        Assert.That(message, Is.InstanceOf<EchoMessage>());
    }

    [Test]
    public void MessageTest()
    {
        var socketEndpoint = Parse("127.0.0.1:6000");
        var connectionEndpoint = Parse("127.0.0.1:6001");
        var connection = Connection.NewConnection(connectionEndpoint);
        var message = new UserMessage("hello", "socket");
        var socket = new Socket(socketEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var cts = new CancellationTokenSource();
        socket.Bind(socketEndpoint);
        connection.StartListen(
            new Action<Message>(actual => Assert.That(actual, Is.EqualTo(new UserMessage("hello", "socket")))),
            cts.Token);
        Thread.Sleep(100);
        socket.Connect(connectionEndpoint);
        socket.Send(message.ToBytes());
        socket.Close();
        cts.Cancel();
        cts.Dispose();
        connection.Dispose();
    }

    [Test]
    public void TwoConnectionsTest()
    {
        var connectionEndpoint = Parse("127.0.0.1:7000");
        var receivedMessages = new List<Message>();
        var sentMessages = new List<Message>() {new EchoMessage(), new UserMessage("hi!", "user2")};
        using var connection1 = Connection.NewConnection(connectionEndpoint);
        using var cts = new CancellationTokenSource();
        connection1.StartListen(
            new Action<Message>(actual => receivedMessages.Add(actual)),
            cts.Token);
        Thread.Sleep(100);
        using var connection2 = Connection.ConnectTo(connectionEndpoint);
        connection2.SendTo((new UserMessage("hi!","user2")).ToBytes(), connectionEndpoint);
        Thread.Sleep(100);
        cts.Cancel();
        Assert.That(receivedMessages, Is.EqualTo(sentMessages));
    }

    [Test]
    public void BroadcastTest()
    {
        var connectionEndpoint = Parse("127.0.0.1:8000");
        using var connection1 = Connection.NewConnection(connectionEndpoint);
        var messages1 = new List<string>();
        using var chat1 = new Chat.Chat(connection1, "user1", messages1, new List<MemberInfo>());
        chat1.Start();
        Thread.Sleep(100);
        using var connection2 = Connection.ConnectTo(connectionEndpoint);
        var messages2 = new List<string>();
        using var chat2 = new Chat.Chat(connection2, "user2", messages2, new List<MemberInfo>());
        chat2.Start();
        Thread.Sleep(100);
        using var connection3 = Connection.ConnectTo(connectionEndpoint);
        var messages3 = new List<string>();
        using var chat3 = new Chat.Chat(connection3, "user3", messages3, new List<MemberInfo>());
        chat3.Start();
        Thread.Sleep(100);
        Assert.Multiple(() =>
        {
            Assert.That(messages1[0], Is.EqualTo("User user2 connected"));
            Assert.That(messages1[1], Is.EqualTo("User user3 connected"));
            Assert.That(messages2[0], Is.EqualTo("User user3 connected"));
        });
        chat3.SendText("hi!");
        Thread.Sleep(100);
        Assert.Multiple(() =>
        {
            Assert.That(messages1.Last(), Is.EqualTo("user3: hi!"));
            Assert.That(messages2.Last(), Is.EqualTo("user3: hi!"));
            Assert.That(messages3.Last(), Is.EqualTo("user3: hi!"));
        });
    }
    
    [Test]
    public void DisconnectTest()
    {
        var connectionEndpoint = Parse("127.0.0.1:9000");
        using var connection1 = Connection.NewConnection(connectionEndpoint);
        var members1 = new List<MemberInfo>();
        using var chat1 = new Chat.Chat(connection1, "user1", new List<string>(), members1);
        chat1.Start();
        Thread.Sleep(100);
        using var connection2 = Connection.ConnectTo(connectionEndpoint);
        var members2 = new List<MemberInfo>();
        using var chat2 = new Chat.Chat(connection2, "user2", new List<string>(),members2);
        chat2.Start();
        Thread.Sleep(100);
        using var connection3 = Connection.ConnectTo(connectionEndpoint);
        var members3 = new List<MemberInfo>();
        var chat3 = new Chat.Chat(connection3, "user3", new List<string>(), members3);
        chat3.Start();
        Thread.Sleep(100);
        chat3.SendText("hi!");
        Thread.Sleep(100);
        Assert.Multiple(() =>
        {
            Assert.That(members1.Count, Is.EqualTo(2));
            Assert.That(members2.Count, Is.EqualTo(2));
            Assert.That(members3.Count, Is.EqualTo(2));
        });
        chat3.Dispose();
        Assert.Multiple(() =>
        {
            Assert.That(members1.Count, Is.EqualTo(1));
            Assert.That(members2.Count, Is.EqualTo(1));
        });
    }
}