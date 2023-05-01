using Microsoft.AspNetCore.Mvc;
using Task4;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var lazySetExamSystem = new ExamSystem(() => new LazySet<ExamInfo>());
var fineGrainedSetExamSystem = new ExamSystem(() => new LazySet<ExamInfo>());

app.MapPost("/lazy", ([FromQuery] long studentId, [FromQuery] long courseId) => lazySetExamSystem.Add(studentId, courseId));
app.MapGet("/lazy", ([FromQuery] long studentId, [FromQuery] long courseId) => lazySetExamSystem.Contains(studentId, courseId));
app.MapDelete("/lazy", ([FromQuery] long studentId, [FromQuery] long courseId) => lazySetExamSystem.Remove(studentId, courseId));
app.MapGet("/lazy/count", () => lazySetExamSystem.Count);

app.MapPost("/fg", ([FromQuery] long studentId, [FromQuery] long courseId) => fineGrainedSetExamSystem.Add(studentId, courseId));
app.MapGet("/fg", ([FromQuery] long studentId, [FromQuery] long courseId) => fineGrainedSetExamSystem.Contains(studentId, courseId));
app.MapDelete("/fg", ([FromQuery] long studentId, [FromQuery] long courseId) => fineGrainedSetExamSystem.Remove(studentId, courseId));
app.MapGet("/fg/count", () => fineGrainedSetExamSystem.Count);

app.Run();
