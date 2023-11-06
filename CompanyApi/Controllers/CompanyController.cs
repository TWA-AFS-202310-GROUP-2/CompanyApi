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

        [HttpDelete]
        public void ClearData()
        { 
            companies.Clear();
        }
    }
}
