using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Xml.Linq;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> Create(CreateCompanyRequest request)
        {
            if (companies.Exists(company => company.Name.Equals(request.Name)))
            {
                return BadRequest();
            }
            Company companyCreated = new Company(request.Name);
            companies.Add(companyCreated);
            return StatusCode(StatusCodes.Status201Created, companyCreated);
        }

        [HttpGet]
        public List<Company> Get()
        {
            return companies;
        }

        [HttpGet("{id}")]
        public Company Get(string id)
        {
            return companies.Where(company => company.Id == id).FirstOrDefault();
        }

        [HttpGet("pageIndex={pageIndex}&pageSize={pageSize}")]
        public async Task<List<Company>> GetByPage(int pageIndex, int pageSize)
        {
            List<Company> newcompanies = GenerateCompanies(pageIndex, pageSize);
            return newcompanies.GetRange((pageIndex - 1) * pageSize, pageSize);
        }

        [HttpPut("{id}")]
        public ActionResult<Company> Put(string id,[FromBody]CreateCompanyRequest createCompanyRequest)
        {
            var index = companies.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                companies[index].Name = createCompanyRequest.Name;
                return companies[index];
            }
            return NotFound();
        }
        [HttpPost("{CompanyId}/employee")]
        public ActionResult<List<Employee>> CreateEmployee(string CompanyId, Employee employee)
        {
            var company = companies.Find(c => c.Id == CompanyId);
            List<Employee> employeesOfCompany = new List<Employee>();
            Employee newemployee = new Employee()
            {
                Name = employee.Name,
                Salary = employee.Salary,
                Company = "company"+ CompanyId,
            };
            employeesOfCompany.Add(newemployee);
            company.employees = employeesOfCompany;
            return StatusCode(StatusCodes.Status201Created, employeesOfCompany);
        }
        [HttpDelete("{CompanyId}/employees/{EmployeeId}")]
        public ActionResult<string> DeleteEmployee(string CompanyId, string EmployeeId)
        {
            Company? company = companies.Where(company => company.Id == CompanyId).FirstOrDefault(); 
            Employee? employeeToDelete = company.employees.Find(e => e.Id == EmployeeId);
            if (employeeToDelete == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            company.employees.Remove(employeeToDelete);
            return StatusCode(StatusCodes.Status204NoContent);
        }

        private static List<Company> GenerateCompanies(int pageIndex, int pageSize)
        {
            List<Company> companies = new List<Company>();
            for (int i = 1; i <= pageIndex * pageSize; i++)
            {
                Company newcompany = new Company($"new company {i}");
                companies.Add(newcompany);
            }

            return companies;
        }

        [HttpDelete]
        public void ClearData()
        { 
            companies.Clear();
        }
    }
}
