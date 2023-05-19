using ExamSystem.Systems;

var usage = $"Usage: {Environment.GetCommandLineArgs()[0]} <System type>";

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
IExamSystem system;

if (args.Length != 1)
{
    throw new ArgumentException($"Expected number of arguments - 1, given - {args.Length}{Environment.NewLine}" +
                            usage);
}

system = args[0] switch
{
    "fine-grained" => new FineGrainedSystem(),
    "lazy" => new LazySystem(),
    _ => throw new ArgumentException($"Expected 'fine-grained' or 'lazy', given - '{args[0]}'{Environment.NewLine}" +
                                     usage)
};

app.MapGet("/", () => "ExamSystem Web API");

app.MapPost("/api",
    (long studentId, long courseId) => system.Add(studentId, courseId));
app.MapDelete("/api",
    (long studentId, long courseId) => system.Remove(studentId, courseId));
app.MapGet("/api",
    (long studentId, long courseId) => system.Contains(studentId, courseId));
app.MapGet("/api/count",
    () => system.Count);

app.Run();