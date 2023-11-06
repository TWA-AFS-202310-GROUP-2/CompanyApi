using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        private static Dictionary<string, Company> companyOne = new Dictionary<string, Company>();

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

        [HttpDelete]
        public void ClearData()
        { 
            companies.Clear();
        }

        [HttpGet]
        public List<Company> GetAll()
        {
            return companies;
        }

        [HttpGet("{name}")]
        public ActionResult<Company> Get(string name)
        {
            foreach (var company in companies)
            {
                if (name == company.Name)
                {
                    return StatusCode(StatusCodes.Status200OK, company);
                }
            }
            
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpGet("{pageSize}/{pageIndex}")]
        public List<Company> ObtainCompaniesFromIndexPageSize(int pageSize, int pageIndex)
        {
            int start_index = pageIndex * pageSize;
            int end_index = start_index + pageSize;
            if (companies.Count > end_index)
            {
                return null;
            }
            else
            {
                companies.GetRange(start_index, pageSize);
                return companies;
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Company> UpdateCompany(string id, CreateCompanyRequest request)
        {
            var company = companies.Find(company => company.Id.Equals(id));
            if (company == null)
            {
                return NotFound();
            }
            company.Name = request.Name;
            return StatusCode(StatusCodes.Status200OK, company);
        }

        [HttpPost("{companyId}/employees")]
        public ActionResult<Company> CreateEmployee(string companyId, CreateEmployeeRequest request)
        {
            Employee employeeCreated = new Employee(request.Name, request.Salary);
            Company? company = companies.Find(company => company.Id == companyId);
            if (company == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            company.Employees.Add(employeeCreated);
            return StatusCode(StatusCodes.Status201Created, employeeCreated);
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult DeleteEmployee(string companyId, string employeeId)
        {
            Company? company = companies.Find(company => companyId.Equals(company.Id));
            if (company != null)
            {
                Employee? employee = company.Employees.Find(employee => employeeId.Equals(employee.Id));
                if (employee != null)
                {
                    company.Employees.Remove(employee);
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

    }
}
