using ExamSystem.ExamSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LazySetController : ControllerBase
    {
        private static readonly LazyExamSystem lazyExSys = new();

        [HttpPost("AddLazy")]
        public void AddLazy(long studentId, long courseId)
        {
            lazyExSys.Add(studentId, courseId);
        }

        [HttpDelete("DeleteLazy")]
        public void RemoveLazy(long studentId, long courseId)
        {
            lazyExSys.Remove(studentId, courseId);
        }

        [HttpGet("ContainsLazy")]
        public bool ContainsLazy(long studentId, long courseId)
        {
            return lazyExSys.Contains(studentId, courseId);
        }

        [HttpGet("CountLazy")]
        public int CountLazy()
        {
            return lazyExSys.Count;
        }
    }
}
