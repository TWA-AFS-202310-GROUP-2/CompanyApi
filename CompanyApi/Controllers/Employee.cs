namespace CompanyApi.Controllers
{
    public class Employee
    { 
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public required int Salary { get; set; }
        public string Company { get; set; }
    }

}