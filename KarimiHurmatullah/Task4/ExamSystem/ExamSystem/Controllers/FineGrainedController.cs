using ExamSystem.ExamSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FineGrainedController : ControllerBase
    {
        private static readonly FineGrainedExamSystem fineGrained = new();

        [HttpPost("AddFineGrained")]
        public void AddFineGrained(long studentId, long courseId)
        {
            fineGrained.Add(studentId, courseId);
        }

        [HttpDelete("DeleteFineGrained")]
        public void RemoveFineGrained(long studentId, long courseId)
        {
            fineGrained.Remove(studentId, courseId);
        }

        [HttpGet("ContainsFineGrained")]
        public bool ContainsFineGrained(long studentId, long courseId)
        {
            return fineGrained.Contains(studentId, courseId);
        }

        [HttpGet("CountFineGrained")]
        public int CountCoarse()
        {
            return fineGrained.Count;
        }
    }
}
