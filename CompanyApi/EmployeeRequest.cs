namespace CompanyApi
{
    public class EmployeeRequest
    {
        public EmployeeRequest(double salary, string name)
        {
            Salary = salary;
            Name = name;
        }

        public double Salary { get; set; }
        public string Name { get; set; }

    }
}
