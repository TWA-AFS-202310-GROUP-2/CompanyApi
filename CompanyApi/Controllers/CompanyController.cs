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
        public List<Company> Get()
        {
            return companies;
        }

        [HttpGet("{name}")]
        public Company Get(string name)
        {
            return companies.Where(company =>company.Name == name).FirstOrDefault();
        }
        [HttpGet("pageIndex={pageIndex}&pageSize={pageSize}")]
        public async Task<List<Company>> GetByPage(int pageIndex, int pageSize)
        {
            List<Company> newcompanies = GenerateCompanies(pageIndex, pageSize);
            return newcompanies.GetRange((pageIndex - 1) * pageSize, pageSize);
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
