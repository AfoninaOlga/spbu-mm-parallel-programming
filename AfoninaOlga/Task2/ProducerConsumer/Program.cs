using ProducerConsumer;

var usage = $"Usage: {Environment.GetCommandLineArgs()[0]} <Number of producers> <Number of consumers>";

if (args.Length != 2
    || !int.TryParse(args[0], out var producersCount)
    || !int.TryParse(args[1], out var consumersCount))
{
    throw new ArgumentException($"Expected number of arguments - 2, given - {args.Length}{Environment.NewLine}" +
                                usage);
}

if (producersCount <= 0 || consumersCount <= 0)
{
    throw new ArgumentException("Arguments must be positive integers");
}

var producers = new Worker<int>[producersCount];
var consumers = new Worker<int>[consumersCount];

var buffer = new Buffer<int>();

for (var i = 0; i < producersCount; ++i)
{
    var localI = i;
    producers[i] = new Producer<int>(localI, buffer, () =>
    {
        Thread.Sleep(1000);
        Console.WriteLine($"Producer #{localI} added {localI} to buffer");
        return localI;
    });
}

for (var i = 0; i < consumersCount; ++i)
{
    var localI = i;
    consumers[i] = new Consumer<int>(localI, buffer, (data) =>
    {
        Console.WriteLine($"Consumer #{localI} took {data} from buffer");
        Thread.Sleep(1500);
    });
}

var workers = producers.Concat(consumers).ToArray();
foreach (var worker in workers)
{
    worker.Start();
}

Console.ReadKey();

foreach (var worker in workers)
{
    worker.Stop();
}

Console.WriteLine("All producers and consumers finished");