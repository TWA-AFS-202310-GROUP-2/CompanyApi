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

        [HttpGet]
        public ActionResult<List<Company>> Get()
        {
            return Ok(companies);
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

            if(oldCompany == null)
            {
                return NotFound();
            }
            else
            {
                oldCompany.Name = company.Name;
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
