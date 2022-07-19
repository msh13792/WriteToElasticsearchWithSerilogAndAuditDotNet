using Audit.Core;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WriteToElasticsearchWithSerilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private static List<Student> _students = new()
        {
            new Student() { StudentId = 1, FirstName = "Rob", LastName = "Justice", DateOfBirth = new DateTime(1970,03,03) },
            new Student() { StudentId = 2, FirstName = "Marybeth", LastName = "Danny", DateOfBirth = new DateTime(1975,07,12) },
        };

        private readonly ILogger<StudentController> _iLogger;

        public StudentController(ILogger<StudentController> iLogger)
        {
            _iLogger = iLogger;
        }

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            _iLogger.LogInformation("Get api called.");
            return _students;
        }

        [HttpPost]
        public void Post([FromBody] PostStudentRequest request)
        {
            Student student = request.ToStudent();
            student.StudentId = _students.Max(x => x.StudentId) + 1;
            student.DateCreated = DateTime.Now;
            _students.Add(student);
            Log.Information("Hello, world! " + student.DateCreated);
        }

        [HttpPut]
        public void Put([FromBody] PutStudentRequest request)
        {
            var entity = _students.FirstOrDefault(x => x.StudentId == request.StudentId);

            if (entity == null)
            {
                throw new ApplicationException("Student not found.");
            }

            using (var audit = AuditScope.Create($"{nameof(Student)}-audit", () => entity, new { ReferenceId = request.StudentId }))
            {

                entity.FirstName = request.FirstName;
                entity.LastName = request.LastName;

                request.UpdateStudent(entity);

                audit.Comment("Data Updated");
            }
        }

        [HttpDelete]
        public void Delete([FromHeader] int studentId)
        {
            Student student = _students.FirstOrDefault(x => x.StudentId == studentId);

            if (student == null)
            {
                throw new ApplicationException("Student not found.");
            }

            _students.Remove(student);
        }
    }
}