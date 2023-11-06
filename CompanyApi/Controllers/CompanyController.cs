using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<List<Company>> GetAllComanies()
        {
            return Ok(companies);
        }

        [HttpGet("{name}")]
        public ActionResult<Company> GetOneComany(string name)
        {
            if (!companies.Exists(company => company.Name.Equals(name)))
            {
                return BadRequest();
            }

            Company company = companies.Find(c => c.Name == name);
            return StatusCode(StatusCodes.Status200OK, company);
        }

        [HttpGet("{PageIndex}/{PageSize}")]
        public ActionResult<List<Company>> GetComanyList(int pageIndex, int pageSize)
        {
            int companyNumberBeforeIndex = pageSize * (pageIndex - 1);
            if (companyNumberBeforeIndex > companies.Count)
            {
                return BadRequest();
            }
            int companyLeft = companies.Count - companyNumberBeforeIndex;
            if (companyLeft > pageSize)
            {
                return Ok(companies.GetRange(companyNumberBeforeIndex, pageSize));
            }
            else
            {
                return Ok(companies.GetRange(companyNumberBeforeIndex, companyLeft));
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Company> UpdateOneCompany(string id, [FromBody] CreateCompanyRequest newCompany)
        {
            if (!companies.Exists(company => company.Id.Equals(id)))
            {
                return BadRequest();
            }

            var updatedCompany = new Company(newCompany.Name);
            companies[companies.IndexOf(companies.Find(cp => cp.Id == id))] = updatedCompany;
            return Ok(updatedCompany);

        }

        [HttpPost("{companyId}")]
        public ActionResult<Company> UpdateEmployeeToCompany(string companyId, [FromBody] Employee employee)
        {
            if (!companies.Exists(company => company.Id.Equals(companyId)))
            {
                return BadRequest();
            }

            var company = companies.Find(w=>w.Id==companyId);
            company.Employee = employee;
            companies[companies.IndexOf(companies.Find(cp => cp.Id == companyId))] = company;
            return Ok(company);

        }

        [HttpDelete]
        public void ClearData()
        {
            companies.Clear();
        }
    }
}
