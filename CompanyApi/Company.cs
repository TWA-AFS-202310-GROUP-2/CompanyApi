namespace CompanyApi
{
    public class Company
    {
        public Company()
        {
            Employee = new Employee();
        }    
        public Company(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Employee = new Employee();
        }

        public Company(string name,string id)
        {
            Id = id;
            Name = name;
            Employee = new Employee();
        }

        public Employee Employee { get; set; }
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class Employee
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Employee() { }

        public Employee(string name)
        {
            Id = Guid.NewGuid().ToString();
        }

        public Employee(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
