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
    }
}
