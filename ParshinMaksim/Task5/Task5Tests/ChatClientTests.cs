using System.Net;
using System.Net.Sockets;
using Task5;

namespace Task5Tests
{
	public class ChatClientTests
	{
		private void OnExceptionOccured(object? sender, Exception e)
		{
			throw e;
		}
		private static int GetFreePort()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}

		[Test]
		[Repeat(3)]
		public async Task StartTest()
		{
			using var client = new ChatClient(new UserInfo("Test"), GetFreePort());
			client.ConnectionExceptionOccured += OnExceptionOccured;
			client.Start();
			Thread.Sleep(500);
			await client.StopAsync();
		}

		[Test]
		[Repeat(3)]
		public async Task ConnectTest()
		{
			var port = GetFreePort();
			using var client = new ChatClient(new UserInfo("Test"), port);
			client.ConnectionExceptionOccured += OnExceptionOccured;
			using var client2 = new ChatClient(new UserInfo("Test2"), GetFreePort());
			client2.ConnectionExceptionOccured += OnExceptionOccured;
			client.Start();
			Thread.Sleep(1000);
			await client2.ConnectAsync(IPEndPoint.Parse($"127.0.0.1:{port}"));

			await client.StopAsync();
			await client2.StopAsync();
		}

		[Test]
		[Repeat(3)]
		public async Task ExchangeMessagesTest()
		{
			var port = GetFreePort();
			using var client = new ChatClient(new UserInfo("Guy"), port);
			client.ConnectionExceptionOccured += OnExceptionOccured;
			using var client2 = new ChatClient(new UserInfo("Mark"), GetFreePort());
			client2.ConnectionExceptionOccured += OnExceptionOccured;
			client.Start();
			Thread.Sleep(1000);
			await client2.ConnectAsync(IPEndPoint.Parse($"127.0.0.1:{port}"));

			const string message = "Oh, hi Mark";
			var messageReceived = false;
			client2.MessageReceived += (_, m) =>
			{
				if (m.Item2 == message)
				{
					messageReceived = true;
				}
			};
			await client.SendMessageAsync(message);

			Thread.Sleep(500);

			await client.StopAsync();
			await client2.StopAsync();

			Assert.That(messageReceived, Is.True);
		}

		[Test]
		[TestCase(5)]
		[TestCase(20)]
		[TestCase(100)]
		public async Task MultipleClientsTest(int clientsNumber)
		{
			var clients = new ChatClient[clientsNumber];
			var receivedMessages = new HashSet<(string, string)>[clientsNumber];
			var firstPort = GetFreePort();

			try
			{
				for (var i = 0; i < clients.Length; i++)
				{
					if (i == 0)
					{
						clients[i] = new ChatClient(new UserInfo($"Guy_{i}"), firstPort, clientsNumber * 2);
					}
					else
					{
						clients[i] = new ChatClient(new UserInfo($"Guy_{i}"), GetFreePort(), clientsNumber * 2);
					}

					clients[i].ConnectionExceptionOccured += OnExceptionOccured;
					receivedMessages[i] = new();
					var currentIndex = i;
					clients[i].MessageReceived += (_, m) => receivedMessages[currentIndex].Add((m.Item1.Name, m.Item2));
				}

				clients[0].Start();
				Thread.Sleep(1000);

				var firstEndPoint = IPEndPoint.Parse($"127.0.0.1:{firstPort}");
				for (var i = 1; i < clients.Length; ++i)
				{
					await clients[i].ConnectAsync(firstEndPoint);
				}


				await Parallel.ForEachAsync(clients, async (c, _) => await c.SendMessageAsync($"Hi"));
				Thread.Sleep(1000);

				for (var i = 0; i < clients.Length; i++)
				{
					Assert.That(receivedMessages[i], Has.Count.EqualTo(clients.Length - 1));

					for (var j = 0; j < clients.Length; j++)
					{
						if (i == j)
						{
							continue;
						}

						Assert.That(receivedMessages[i].Contains(($"Guy_{j}", "Hi")), Is.True);
					}
				}
			}
			finally
			{
				for (var i = 0; i < clients.Length; i++)
				{
					await clients[i].StopAsync();
				}

				for (var i = 0; i < clients.Length; i++)
				{
					clients[i]?.Dispose();
				}
			}
		}
	}
}
