using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
        public ActionResult<List<Company>> Get(string? pageSize, string? pageIndex)
        {
            if (pageSize == null || pageIndex == null)
            {
                return Ok(companies);
            }
            else
            {
                return companies.Skip(int.Parse(pageSize) * (int.Parse(pageIndex) - 1)).Take(int.Parse(pageSize)).ToList();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Company> Get(string id)
        {
            var company = companies.Where(c => c.Id == id).FirstOrDefault();
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(company);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Company> Put(string id, CreateCompanyRequest company)
        {
            var oldCompany = companies.Where(c => c.Id == id).FirstOrDefault();

            if (oldCompany == null)
            {
                return NotFound();
            }
            else
            {
                oldCompany.Name = company.Name;
                return NoContent();
            }
        }

        [HttpPost("{companyId}/employees")]
        public ActionResult<Employee> AddEmployee(string companyId, EmployeeRequest employee)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                Employee newEmployee = new Employee();
                newEmployee.Salary = employee.Salary;
                newEmployee.Name = employee.Name;
                company.Employees.Add(newEmployee);

                return Created("", newEmployee);
            }
        }

        [HttpGet("{companyId}/employees")]
        public ActionResult<List<Employee>> GetEmployeesByCompanyId(string companyId)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(company.Employees);
            }
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult DeleteEmployee(string companyId, string employeeId)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            var employee = company?.Employees.FirstOrDefault(e => e.Id == employeeId);
            if(employee == null)
            {
                return NotFound();
            }
            else
            {
                company.Employees.Remove(employee);
                return NoContent();
            }
        }

        [HttpPut("{companyId}/employees/{employeeId}")]
        public ActionResult UpdateEmployee(string companyId, string employeeId, EmployeeRequest employeeRequest)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            var employee = company?.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                employee.Salary = employeeRequest.Salary;
                employee.Name = employeeRequest.Name;
                return NoContent();
            }
        }

        [HttpDelete]
        public void ClearData()
        {
            companies.Clear();
        }
    }
}
