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
var consumerTasks = new Task[consumersCount];
var producerTasks = new Task[producersCount];
var random = new Random();
var token = new CancellationTokenSource();

for (var i = 0; i < producersCount; ++i)
{
    var id = i;

    int Produce()
    {
        Thread.Sleep(random.Next(100, 2000));
        Console.WriteLine($"Produce {id} from {id}");
        return id;
    }

    var producer = new Producer<int>(Produce);
    var producerTask = task.AddProducer(producer, token.Token);
    producerTask.Start();
    producerTasks[i] = producerTask;
}

for (var i = 0; i < consumersCount; ++i)
{
    var id = i;

    void Consume(int value)
    {
        Console.WriteLine($"Consume {value} from {id}");
        Thread.Sleep(random.Next(100, 2000));
    }

    var consumer = new Consumer<int>(Consume);
    var consumerTask = task.AddConsumer(consumer, token.Token);
    consumerTask.Start();
    consumerTasks[i] = consumerTask;
}

Console.WriteLine("Press any key to stop the process");

Console.ReadKey();

token.Cancel();

Task.WaitAll(consumerTasks, token.Token);
Task.WaitAll(producerTasks, token.Token);

Console.WriteLine("Exited");
