using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpGet]
        public ActionResult<List<Company>> GetAll()
        {
            return StatusCode(StatusCodes.Status200OK, companies);
        }

        [HttpGet("{Id}")]
        public ActionResult<Company> GetById(string Id)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == Id)
                {
                    return StatusCode(StatusCodes.Status200OK, companies[i]);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpGet("{pageSize}/{pageIndex}")]
        public ActionResult<List<Company>> GetByPageInfo(int pageSize, int pageIndex)
        {
            List<Company> resCompanies = new List<Company>();

            for (int i = pageSize * pageIndex; i < pageSize * (pageIndex + 1); i++)
            {
                if (i == companies.Count) break;
                resCompanies.Add(companies[i]);
            }
            return StatusCode(StatusCodes.Status200OK, resCompanies);
        }

        [HttpGet("{Id}/employees")]
        public ActionResult<Employee> GetEmployees(string Id)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == Id)
                {
                    return StatusCode(StatusCodes.Status201Created, companies[i].EmployeeList);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut("{Id}")]
        public ActionResult<Company> Put(string Id, CreateCompanyRequest request)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == Id)
                {
                    companies[i].Name = request.Name;
                    return StatusCode(StatusCodes.Status200OK, companies[i]);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPut("{CompanyId}/employees/{EmployeeId}")]
        public ActionResult<Employee> PutEmployee(string CompanyId, string EmployeeId, CreateEmployeeRequest request)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == CompanyId)
                {
                    for (int j = 0; j < companies[i].EmployeeList.Count; j++)
                    {
                        if (companies[i].EmployeeList[j].Id == EmployeeId)
                        {
                            companies[i].EmployeeList[j].Name = request.Name;
                            companies[i].EmployeeList[j].Salary = request.Salary;
                            return StatusCode(StatusCodes.Status200OK, companies[i].EmployeeList[j]);
                        }
                    }
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

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

        [HttpPost("{Id}/employees")]
        public ActionResult<Employee> CreateEmployee(string Id, CreateEmployeeRequest request)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == Id)
                {
                    Employee employeeCreated = new Employee(request.Name, request.Salary);
                    companies[i].EmployeeList.Add(employeeCreated);
                    return StatusCode(StatusCodes.Status201Created, employeeCreated);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete]
        public void ClearData()
        {
            companies.Clear();
        }

        [HttpDelete("{CompanyId}")]
        public ActionResult<string> DeleteCompany(string CompanyId)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == CompanyId)
                {
                    companies.RemoveAt(i);
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }

            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpDelete("{CompanyId}/employees/{EmployeeId}")]
        public ActionResult<string> DeleteEmployee(string CompanyId, string EmployeeId)
        {
            for (int i = 0; i < companies.Count; i++)
            {
                if (companies[i].Id == CompanyId)
                {
                    for (int j = 0; i < companies[i].EmployeeList.Count; j++)
                    {
                        if (companies[i].EmployeeList[j].Id == EmployeeId)
                        {
                            companies[i].EmployeeList.RemoveAt(j);
                            return StatusCode(StatusCodes.Status204NoContent);
                        }
                    }
                }
            }

            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}
