using System.Buffers.Binary;
using System.Net;
using System.Text;

namespace Task5
{
	internal class Protocol
	{
		public const int HeaderLength = 5;

		public static (UserInfo, string) BytesToMessage(byte[] bytes)
		{
			var raw = Encoding.UTF8.GetString(bytes);
			var messageWithUserInfo = raw.Split('$');
			return (new UserInfo(messageWithUserInfo[0]), messageWithUserInfo[1]);
		}

		public static (UserInfo, IPEndPoint) BytesToAcknowledgement(byte[] bytes)
		{
			var endPointWithUserInfo = Encoding.UTF8.GetString(bytes).Split('$');
			return (new UserInfo(endPointWithUserInfo[0]), IPEndPoint.Parse(endPointWithUserInfo[1]));
		}

		public static IEnumerable<IPEndPoint> BytesToPeerList(byte[] bytes)
		{
			var peerListString = Encoding.UTF8.GetString(bytes);
			if (peerListString == string.Empty)
			{
				return Enumerable.Empty<IPEndPoint>();
			}
			var endpointStrings = peerListString.Split(';');
			var peerList = new List<IPEndPoint>();
			foreach (var peer in endpointStrings)
			{
				var endpoint = IPEndPoint.Parse(peer);
				peerList.Add(endpoint);
			}
			return peerList;
		}

		public static IPEndPoint BytesToPeerListRequest(byte[] bytes)
		{
			var endPointString = Encoding.UTF8.GetString(bytes);
			return IPEndPoint.Parse(endPointString);
		}

		public static byte[] GetHeader(DataType dataType, int bytesToSend)
		{
			var header = new byte[5];
			header[0] = (byte)dataType;
			var span = new Span<byte>(header, 1, 4);
			BinaryPrimitives.WriteInt32BigEndian(span, bytesToSend);
			return header;
		}

		public static (DataType, int) ParseHeader(Span<byte> header)
		{
			var dataType = (DataType)header[0];
			var length = BinaryPrimitives.ReadInt32BigEndian(header[1..5]);
			return (dataType, length);
		}

		public static byte[] MessageToBytes(UserInfo info, string message)
		{
			var messageWithUserName = $"{info.Name}${message}";
			return Encoding.UTF8.GetBytes(messageWithUserName);
		}

		public static byte[] AcknowledgementToBytes(UserInfo info, IPEndPoint endPoint)
		{
			var endPointWithUserName = $"{info.Name}${endPoint.Address}:{endPoint.Port}";
			return Encoding.UTF8.GetBytes(endPointWithUserName);
		}

		public static byte[] PeerListRequestToBytes(IPEndPoint endPoint)
		{
			return Encoding.UTF8.GetBytes(endPoint.ToString());
		}

		public static byte[] PeerListToBytes(IEnumerable<IPEndPoint> peers)
		{
			var peerListStringBuilder = new StringBuilder();
			var peersArray = peers.ToArray();

			for (var i = 0; i < peersArray.Length; ++i)
			{
				peerListStringBuilder = peerListStringBuilder.Append($"{peersArray[i].Address}:{peersArray[i].Port}");
				if (i != peersArray.Length - 1)
				{
					peerListStringBuilder.Append(';');
				}
			}

			return Encoding.UTF8.GetBytes(peerListStringBuilder.ToString());
		}
	}
}
