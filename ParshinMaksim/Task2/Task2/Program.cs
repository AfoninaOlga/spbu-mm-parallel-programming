using Task2;

if (args.Length != 2)
{
    throw new ArgumentException("There should be exactly two command line arguments: number of producers and number of consumers");
}

if (!int.TryParse(args[0], out var producersCount) || producersCount <= 0)
{
    throw new ArgumentException("Number of producers should be a positive integer");
}

if (!int.TryParse(args[1], out var consumersCount) || consumersCount <= 0)
{
    throw new ArgumentException("Number of consumers should be a positive integer");
}

var task = new ProducerConsumerTask<int>();
var consumers = new List<IConsumer>();
var producers = new List<IProducer>();
var random = new Random();

for (var i = 0; i < producersCount; ++i)
{
    var id = i;

    int Produce()
    {
        Thread.Sleep(random.Next(100, 4000));
        Console.WriteLine($"Produce {id} from {id}");
        return id;
    }

    var producer = task.AddProducer(Produce);
    producers.Add(producer);
}

for (var i = 0; i < consumersCount; ++i)
{
    var id = i;

    void Consume(int value)
    {
        Console.WriteLine($"Consume {value} from {id}");
        Thread.Sleep(random.Next(100, 4000));
    }

    var consumer = task.AddConsumer(Consume);
    consumers.Add(consumer);
}

var token = new CancellationTokenSource();

Console.WriteLine("Press any key to stop the process");

var consumersTasks = consumers.Select(c => c.Start(token.Token)).ToArray();
var producersTasks = producers.Select(c => c.Start(token.Token)).ToArray();

Console.ReadKey();

token.Cancel();

Task.WaitAll(consumersTasks);
Task.WaitAll(producersTasks);

Console.WriteLine("Exited");
