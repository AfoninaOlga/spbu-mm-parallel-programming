using System;

namespace Task5Ui
{
	internal record class Message(string Sender, DateTime Date, string Text, bool IsMine, bool IsConnectedMessage);
}
