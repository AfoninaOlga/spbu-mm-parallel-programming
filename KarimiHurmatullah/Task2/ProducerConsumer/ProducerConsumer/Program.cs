namespace ProducerConsumer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var pc = new Connector.Connector();
            await pc.Run();
        }
    }
}