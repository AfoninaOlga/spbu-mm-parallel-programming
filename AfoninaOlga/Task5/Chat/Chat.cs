using System.Net;
using System.Net.Sockets;
using Chat.NetworkMessage;

namespace Chat;

public class Chat : IDisposable
{
    public Chat(Connection connection, string username, IList<string> messages, IList<MemberInfo> members)
    {
        _tokenSource = new CancellationTokenSource();
        _token = _tokenSource.Token;
        _connection = connection;
        Me = new MemberInfo(username, _connection.LocalEndPoint.Address.ToString(), _connection.LocalEndPoint.Port);
        _members = members;
        _chatMessages = messages;
    }

    private readonly CancellationTokenSource _tokenSource;
    private readonly Connection _connection;
    private readonly CancellationToken _token;

    private readonly object _lockObj = new();

    private readonly IList<MemberInfo> _members;

    // private readonly IList<ChatMessage> _chatMessages;
    private readonly IList<string> _chatMessages;

    public MemberInfo Me { get; }


    public void Start()
    {
        _connection.StartListen(OnNewIncomeConnection, _token);
        _connection.SendBroadcast(new ConnectMessage(Me).ToBytes());
    }

    private void OnNewIncomeConnection(Message message)
    {
        switch (message)
        {
            case AddMemberMessage addMemberMessage:
                ProcessAddMemberMessage(addMemberMessage);
                Console.WriteLine(
                    $"Process AddMemberMessage [{addMemberMessage.Member.Username}, {addMemberMessage.Member.Ip}, {addMemberMessage.Member.Port}]");
                break;
            case UserMessage userMessage:
                ProcessUserMessage(userMessage);
                Console.WriteLine($"Process UserMessage [{userMessage.Username}, {userMessage.Text}]");
                break;
            case ConnectMessage connectMessage:
                ProcessConnectMessage(connectMessage);
                Console.WriteLine(
                    $"Process ConnectMessage [{connectMessage.NewMember.Username}, {connectMessage.NewMember.Ip}, {connectMessage.NewMember.Port}]");
                break;
            case DisconnectMessage disconnectMessage:
                ProcessDisconnectMessage(disconnectMessage);
                Console.WriteLine(
                    $"Process DisconnectMessage [{disconnectMessage.DisconnectedMember.Username}, {disconnectMessage.DisconnectedMember.Ip}, {disconnectMessage.DisconnectedMember.Port}]");
                break;
            case MembersMessage membersMessage:
                ProcessMembersMessage(membersMessage);
                Console.WriteLine($"Process MembersMessage [{membersMessage.Members.Length}]");
                break;
            case EchoMessage echo:
                Console.WriteLine($"Process Echo");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(message));
        }
    }

    public void SendText(string message)
    {
        _chatMessages.Add(new ChatMessage.TextMessage(Me.Username, message).ToText());
        _connection.SendBroadcast(new UserMessage(message, Me.Username).ToBytes());
    }

    private void ProcessUserMessage(UserMessage msg)
    {
        lock (_lockObj)
        {
            _chatMessages.Add(new ChatMessage.TextMessage(msg.Username, msg.Text).ToText());
        }
    }

    private void ProcessConnectMessage(ConnectMessage msg)
    {
        MemberInfo[] oldMembers;
        var clientEndPoint = new IPEndPoint(IPAddress.Parse(msg.NewMember.Ip), msg.NewMember.Port);
        lock (_lockObj)
        {
            oldMembers = _members.ToArray();
            _connection.AddConnectedClient(clientEndPoint);
        }

        _connection.SendTo(
            new MembersMessage(oldMembers.Concat(new[] { Me }).ToArray()).ToBytes(),
            clientEndPoint
        );
    }

    private void ProcessDisconnectMessage(DisconnectMessage msg)
    {
        lock (_lockObj)
        {
            _members.Remove(_members.First(member =>
                member.Ip == msg.DisconnectedMember.Ip && member.Port == msg.DisconnectedMember.Port));
            _chatMessages.Add(new ChatMessage.DisconnectUserMessage(msg.DisconnectedMember.Username).ToText());
            _connection.RemoveConnectedClient(new IPEndPoint(IPAddress.Parse(msg.DisconnectedMember.Ip),
                msg.DisconnectedMember.Port));
        }
    }

    private void ProcessMembersMessage(MembersMessage msg)
    {
        lock (_lockObj)
        {
            foreach (var member in msg.Members)
            {
                _members.Add(member);
                _connection.AddConnectedClient(new IPEndPoint(IPAddress.Parse(member.Ip), member.Port));
            }
        }

        foreach (var member in msg.Members)
        {
            _connection.SendTo(
                new AddMemberMessage(Me).ToBytes(),
                new IPEndPoint(IPAddress.Parse(member.Ip), member.Port)
            );
        }
    }

    private void ProcessAddMemberMessage(AddMemberMessage msg)
    {
        lock (_lockObj)
        {
            _members.Add(msg.Member);
            _chatMessages.Add(new ChatMessage.ConnectUserMessage(msg.Member.Username).ToText());
            _connection.AddConnectedClient(new IPEndPoint(IPAddress.Parse(msg.Member.Ip), msg.Member.Port));
        }
    }

    public void Dispose()
    {
        _connection.SendBroadcast(new DisconnectMessage(Me).ToBytes());
        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _connection.Dispose();
    }
}
