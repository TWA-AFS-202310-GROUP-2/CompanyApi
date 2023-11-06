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

        [HttpGet("{name}")]
        public Company Get(string name)
        {
            return companies.Where(company => company.Name == name).FirstOrDefault();
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
        [HttpPost("{name}/employee")]
        public ActionResult<List<Employee>> CreateEmployee(string name, Employee employee)
        {
            //if (!companies.Exists(company => company.Name.Equals(name)))
            //{
            //    return BadRequest();
            //}
            List<Employee> employeesOfCompany = new List<Employee>();
            Employee newemployee = new Employee()
            {
                Name = employee.Name,
                Salary = employee.Salary,
                Company = name,
            };
            employeesOfCompany.Add(newemployee);
            return StatusCode(StatusCodes.Status201Created, employeesOfCompany);
        }
        [HttpDelete("{name}/employee/{emplyeeName}")]
        public ActionResult<Company> DeleteEmployee(string name,string employeeName)
        {
            Company? company = companies.Find(c => c.Name == name);
            var employees = company.employees;
            Employee? employeeToDelete =employees.Find(e => e.Name == employeeName);
            if (employeeToDelete == null)
            {
                return NotFound();
            }
            company.employees.Remove(employeeToDelete);
            return Ok(company);
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
