using System.Net;
using System.Net.Sockets;

namespace Task5
{
	public class ChatClient : IDisposable
	{
		private readonly int listenerBacklog;
		private readonly Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		private readonly UserInfo myUserInfo;
		private readonly IPEndPoint myEndPoint;

		private LazySet<string> peers = new();
		private CancellationTokenSource cts = new();
		private Task receiveMessagesTask = Task.CompletedTask;

		public event EventHandler<(UserInfo, string)> MessageReceived;
		public event EventHandler<UserInfo> NewUserConnected;
		public event EventHandler<Exception> ConnectionExceptionOccured;

		public bool IsRunning { get; private set; } = false;

		public ChatClient(UserInfo userInfo, int port, int listenerBacklog = 20)
		{
			myUserInfo = userInfo;
			MessageReceived += (_, _) => { };
			NewUserConnected += (_, _) => { };
			ConnectionExceptionOccured += (_, _) => { };

			var localhost = Dns.GetHostEntry("localhost");
			var myIp = localhost.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
			myEndPoint = new IPEndPoint(myIp, port);
			myUserInfo = userInfo;
			this.listenerBacklog = listenerBacklog;

			listener.Bind(myEndPoint);
		}

		private byte[] GetPacket(DataType dataType, byte[] bytes)
		{
			var header = Protocol.GetHeader(dataType, bytes.Length);
			var packet = new byte[header.Length + bytes.Length];
			Array.Copy(header, packet, header.Length);

			if (bytes.Length > 0)
			{
				Array.Copy(bytes, 0, packet, header.Length, bytes.Length);
			}

			return packet;
		}

		private async Task<(DataType, byte[])> ReceiveBytesAsync(Socket socket)
		{
			var headerBuffer = new byte[Protocol.HeaderLength];
			var headerMemory = new Memory<byte>(headerBuffer);
			await socket.ReceiveAsync(headerMemory, cts.Token);
			var (dataType, messageLength) = Protocol.ParseHeader(headerBuffer);

			var buffer = new byte[messageLength];

			if (messageLength > 0)
			{
				await socket.ReceiveAsync(new Memory<byte>(buffer), cts.Token);
			}

			return (dataType, buffer);
		}

		private async Task SendPacketToPeerAsync(IPEndPoint endPoint, byte[] packet, CancellationToken cancellationToken)
		{
			try
			{
				using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				await socket.ConnectAsync(endPoint, cancellationToken);
				await socket.SendAsync(packet, cancellationToken);
				await socket.DisconnectAsync(false, cancellationToken);
			}
			catch (SocketException)
			{
				peers.Remove(endPoint.ToString());
			}
		}

		private async Task BroadcastAsync(byte[] bytes)
		{
			var peerList = peers.ToList().Select(IPEndPoint.Parse);
			await Parallel.ForEachAsync(
				peerList,
				cts.Token,
				async (endpoint, cancallationToken) => await SendPacketToPeerAsync(endpoint, bytes, cancallationToken));
		}

		private async Task ReceiveMessages()
		{
			listener.Listen(listenerBacklog);

			while (!cts.IsCancellationRequested)
			{
				try
				{
					using var peerSocket = await listener.AcceptAsync(cts.Token);
					var (dataType, data) = await ReceiveBytesAsync(peerSocket);

					switch (dataType)
					{
						case DataType.PeerListRequest:
							{
								var peerListBytes = Protocol.PeerListToBytes(peers.ToList().Select(IPEndPoint.Parse));
								var packet = GetPacket(DataType.PeerList, peerListBytes);
								await peerSocket.SendAsync(packet, cts.Token);

								var endPoint = Protocol.BytesToPeerListRequest(data);
								peers.Add(endPoint.ToString());
								break;
							}
						case DataType.Acknowledgement:
							{
								var (userInfo, endPoint) = Protocol.BytesToAcknowledgement(data);
								peers.Add(endPoint.ToString());
								NewUserConnected(this, userInfo);
								break;
							}
						case DataType.Message:
							{
								var userInfoAndMessage = Protocol.BytesToMessage(data);
								MessageReceived(this, userInfoAndMessage);
								break;
							}
						default:
							{
								break;
							}
					}

					await peerSocket.DisconnectAsync(false, cts.Token);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (SocketException e)
				{
					ConnectionExceptionOccured(this, e);
				}
			}
		}

		/// <summary>
		/// Starts chat client and creates a new chat room
		/// </summary>
		public void Start()
		{
			if (IsRunning)
			{
				throw new InvalidOperationException("Chat client is already started");
			}

			IsRunning = true;
			cts = new();
			receiveMessagesTask = ReceiveMessages();
		}

		/// <summary>
		/// Starts chat client and connects to the existing chat room identifed
		/// by a peer endpoint
		/// </summary>
		public async Task ConnectAsync(IPEndPoint initialPeer)
		{
			if (IsRunning)
			{
				throw new InvalidOperationException("Chat client is already started");
			}

			IsRunning = true;
			cts = new();

			try
			{
				peers.Add(initialPeer.ToString());
				using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				await socket.ConnectAsync(initialPeer, cts.Token);
				var requestBytes = Protocol.PeerListRequestToBytes(myEndPoint);
				var packet = GetPacket(DataType.PeerListRequest, requestBytes);
				await socket.SendAsync(packet, cts.Token);
				var (dataType, data) = await ReceiveBytesAsync(socket);
				await socket.DisconnectAsync(false, cts.Token);

				var peerList = Protocol.BytesToPeerList(data);
				foreach (var endPoint in peerList)
				{
					peers.Add(endPoint.ToString());
				}

				var acknowledgementBytes = Protocol.AcknowledgementToBytes(myUserInfo, myEndPoint);
				var acknowledgementPacket = GetPacket(DataType.Acknowledgement, acknowledgementBytes);
				await BroadcastAsync(acknowledgementPacket);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (SocketException e)
			{
				ConnectionExceptionOccured(this, e);
				return;
			}

			receiveMessagesTask = ReceiveMessages();
		}

		/// <summary>
		/// Sends message to all peers in the session
		/// </summary>
		public async Task SendMessageAsync(string message)
		{
			if (!IsRunning)
			{
				throw new InvalidOperationException("Chat client is not started");
			}

			try
			{
				var messageBytes = Protocol.MessageToBytes(myUserInfo, message);
				var packet = GetPacket(DataType.Message, messageBytes);
				await BroadcastAsync(packet);
			}
			catch (OperationCanceledException)
			{ }
			catch (SocketException e)
			{
				ConnectionExceptionOccured(this, e);
			}
		}

		/// <summary>
		/// Gracefully stops the client with reuse ability
		/// </summary>
		public async Task StopAsync()
		{
			cts.Cancel();
			await receiveMessagesTask;
			IsRunning = false;
		}

		public void Dispose()
		{
			cts.Cancel();
			listener.Dispose();
		}
	}
}
