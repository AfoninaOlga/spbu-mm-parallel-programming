using ExamSystem.ExamSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoarseSetController : ControllerBase
    {
        private static readonly CoarseExamSystem coarseExSys = new();

        [HttpPost("AddCoarse")]
        public void AddCoarse(long studentId, long courseId)
        {
            coarseExSys.Add(studentId, courseId);
        }

        [HttpDelete("DeleteCoarse")]
        public void RemoveCoarse(long studentId, long courseId)
        {
            coarseExSys.Remove(studentId, courseId);
        }

        [HttpGet("ContainsCoarse")]
        public bool ContainsCoarse(long studentId, long courseId)
        {
            return coarseExSys.Contains(studentId, courseId);
        }

        [HttpGet("CountCoarse")]
        public int CountCoarse()
        {
            return coarseExSys.Count;
        }
    }
}
