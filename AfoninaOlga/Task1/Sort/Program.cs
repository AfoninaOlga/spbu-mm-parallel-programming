using Sort;

var usage = $"Usage: {Environment.GetCommandLineArgs()[0]} <Path to file with array> <Path to output file>";

if (args.Length != 2)
    throw new ArgumentException($"Expected number of arguments - 2, given - {args.Length}{Environment.NewLine}" +
                                usage);
var inputPath = args[0];
var outputPath = args[1];

if (!File.Exists(inputPath))
{
    throw new ArgumentException($"Input file {inputPath} doesn't exists");
}

MPI.Environment.Run(ref args, communicator =>
{
    var sorter = new OddEvenSorter(communicator);
    if (communicator.IsMainProcess())
    {
        var array = Utils.ReadArray(inputPath);
        array = sorter.Sort(array);
        if (array != null)
        {
            Utils.WriteArray(array, outputPath);
        }
        else
        {
            throw new Exception("Result in the main process must not be null");
        }
    }
    else
    {
        sorter.Sort(null);
    }
});
