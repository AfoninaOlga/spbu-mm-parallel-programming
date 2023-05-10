using DeansOffice.Sets;
using Microsoft.AspNetCore.Mvc;

namespace DeansOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeansOfficeController : ControllerBase
    {
        private readonly ILogger<DeansOfficeController> _logger;
        private static readonly FineGrainedSet _fineGrainedSet = new();
        private static readonly LazySet _lazySet = new();


        public DeansOfficeController(ILogger<DeansOfficeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/add")]
        [Route("/v1/add")]
        public void Add(long studentId, long courseId)
        {
            _fineGrainedSet.Add(studentId, courseId);
        }

        [HttpPost]
        [Route("/v2/add")]
        public void AddLazy(long studentId, long courseId)
        {
            _lazySet.Add(studentId, courseId);
        }

        [HttpPost]
        [Route("/remove")]
        [Route("/v1/remove")]
        public void Remove(long studentId, long courseId)
        {
            _fineGrainedSet.Remove(studentId, courseId);
        }

        [HttpPost]
        [Route("/v2/remove")]
        public void RemoveLazy(long studentId, long courseId)
        {
            _lazySet.Remove(studentId, courseId);
        }

        [HttpGet]
        [Route("/contains")]
        [Route("/v1/contains")]
        public bool Contains(long studentId, long courseId)
        {
            return _fineGrainedSet.Contains(studentId, courseId);
        }

        [HttpGet]
        [Route("/v2/contains")]
        public bool ContainsLazy(long studentId, long courseId)
        {
            return _lazySet.Contains(studentId, courseId);
        }

        [HttpGet]
        [Route("/count")]
        [Route("/v1/count")]
        public int Count()
        {
            return _fineGrainedSet.Count;
        }

        [HttpGet]
        [Route("/v2/count")]
        public int CountLazy()
        {
            return _lazySet.Count;
        }
    }
}