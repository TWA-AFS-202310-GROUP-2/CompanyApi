namespace CompanyApi
{
    public class Company
    {
        public Company()
        {
        }

        public Company(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
