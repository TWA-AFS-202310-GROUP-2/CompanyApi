namespace CompanyApi
{
    public class Employee
    {
        //TODO: when set id private will be a bug, but i don't know why
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public string CompanyId { get; private set; } 
        public Employee(string name, decimal salary, string companyId)
        {
            Id = Guid.NewGuid().ToString(); 
            Name = name;
            Salary = salary;
            CompanyId = companyId;
        }
    }

}