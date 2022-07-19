namespace WriteToElasticsearchWithSerilog
{
    public class PostStudentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Student ToStudent()
        {
            return new Student()
            {
                FirstName = FirstName,
                LastName = LastName,
                DateOfBirth = DateOfBirth
            };
        }
    }
}