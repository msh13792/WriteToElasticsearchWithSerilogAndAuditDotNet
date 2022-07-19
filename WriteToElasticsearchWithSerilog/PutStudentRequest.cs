namespace WriteToElasticsearchWithSerilog
{
    public class PutStudentRequest
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public void UpdateStudent(Student student)
        {
            student.FirstName = FirstName;
            student.LastName = LastName;
            student.DateOfBirth = DateOfBirth;
            student.DateModified = DateTime.Now;
        }
    }
}