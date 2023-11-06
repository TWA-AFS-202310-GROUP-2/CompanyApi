namespace CompanyApi
{
    public class Employee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double Salary { get; set; }
    }
}
