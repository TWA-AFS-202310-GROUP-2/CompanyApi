using Microsoft.AspNetCore.Mvc;

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

        // [HttpGet]
        // public ActionResult<List<Company>> GetAll()
        // {
        //     return Ok(companies);
        // }

        [HttpGet("{id}")]
        public ActionResult<Company> GetById(string id)
        {
            if (Guid.TryParse(id, out Guid guid) == false || guid == Guid.Empty )
            {
                return BadRequest();
            }
            Company? company = companies.Find(company => company.Id.Equals(id));
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetByPage([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            if (pageIndex > 0 && pageSize > 0)
            {
                var pagedCompanies = companies
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                return Ok(pagedCompanies);
            } else if (pageIndex == 0 && pageSize == 0)
            {
                return Ok(companies);
            } else
            {
                return Ok(new List<Company>());
            }
        }
        [HttpPut("{id}")]
        public ActionResult<Company> Update(string id, [FromBody] UpdateCompanyRequest request)
        {
            var company = companies.FirstOrDefault(c => c.Id == id);
            if (company == null) return NotFound();
            
            company.Name = request.Name;
            companies[companies.FindIndex(c => c.Id == id)] = company;
            return Ok(company);
        }

        [HttpPost("{companyId}/employees")]
        public ActionResult<Employee> AddEmployee(string companyId, [FromBody] CreateEmployeeRequest request)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null) return NotFound();

            Employee employee = new(request.Name, request.Salary, request.CompanyId);
            company.AddEmployee(employee);
            return StatusCode(StatusCodes.Status201Created, employee);
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult DeleteEmployee(string companyId, string employeeId)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null) return NotFound();

            var employee = company.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null) return NotFound();

            company.Employees.Remove(employee);
            return NoContent();
        }
        [HttpGet("{companyId}/employees")]
        public ActionResult<List<Employee>> GetAllEmployees(string companyId)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null) return NotFound();

            return Ok(company.Employees);
        }



        [HttpDelete]
        public void ClearData()
        { 
            companies.Clear();
        }
    }
}
