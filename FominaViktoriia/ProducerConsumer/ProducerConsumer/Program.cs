namespace ProducerConsumer;

public class Program
{
    public static void Main(string[] args)
    {
        // Arg 0: producers size.
        // Arg 1: consumers size.

        Console.WriteLine("""
            Simple console Producer-Consumer task

            arg 0: producers size
            arg 1: consumers size
        """);
        if (args.Length != 2)
        {
            throw new ArgumentException("Command line arguments size must be equal 2!");
        }

        var algorithm = new ProducerConsumerAlgorithm<int>(int.Parse(args[0]), int.Parse(args[1]), () => 5);
        algorithm.Run();
    }
}