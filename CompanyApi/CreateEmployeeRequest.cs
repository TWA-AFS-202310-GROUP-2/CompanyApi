namespace CompanyApi
{
    public class CreateEmployeeRequest
    {
        public required string Name { get; set; }
        public required int Salary { get; set; }
    }
}
