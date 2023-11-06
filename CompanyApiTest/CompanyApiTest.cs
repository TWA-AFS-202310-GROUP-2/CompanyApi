using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace CompanyApiTest
{
    public class CompanyApiTest
    {
        private HttpClient httpClient;

        public CompanyApiTest()
        {
            WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Should_return_created_company_with_status_201_when_create_cpmoany_given_a_company_name()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");
            
            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "api/companies", 
                SerializeObjectToContent(companyGiven)
            );
           
            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            //Company? companyCreated = await DeserializeTo<Company>(httpResponseMessage);
            Company companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            Assert.Equal(companyGiven.Name, companyCreated.Name);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_company_given_a_existed_company_name()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            await httpClient.PostAsync("/api/companies", SerializeObjectToContent(companyGiven));
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies", 
                SerializeObjectToContent(companyGiven)
            );
            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_company_given_a_company_with_unknown_field()
        {
            // Given
            await ClearDataAsync();
            StringContent content = new StringContent("{\"unknownField\": \"BlueSky Digital Media\"}", Encoding.UTF8, "application/json");
          
            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/api/companies", content);
           
            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        private async Task<T?> DeserializeTo<T>(HttpResponseMessage httpResponseMessage)
        {
            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            T? deserializedObject = JsonConvert.DeserializeObject<T>(response);
            return deserializedObject;
        }

        private static StringContent SerializeObjectToContent<T>(T objectGiven)
        {
            return new StringContent(JsonConvert.SerializeObject(objectGiven), Encoding.UTF8, "application/json");
        }

        private async Task ClearDataAsync()
        {
            await httpClient.DeleteAsync("/api/companies");
        }


        [Fact]
        public async Task Should_return_all_companies_with_status_200_when_get_all()
        {
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("api/companies",SerializeObjectToContent(companyGiven));
            Company companyGiven2 = new Company("RedSky Digital Media");
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync("api/companies", SerializeObjectToContent(companyGiven2));
            
            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync("api/companies");
            List<Company> companies = await DeserializeTo<List<Company>>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            Assert.Equal(companyGiven2.Name, companies[1].Name);
        }

        [Fact]
        public async Task Should_return_the_company_with_status_200_when_get_company_given_company_name()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("Blue");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("api/companies", SerializeObjectToContent(companyGiven));

            Company companyGiven2 = new Company("Sky");

            // When
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync("api/companies", SerializeObjectToContent(companyGiven2));

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync("api/companies/Sky");

            // Then
            Company? company = await DeserializeTo<Company>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            Assert.Equal(companyGiven2.Name, company.Name);
        }

        [Fact]
        public async Task Should_return_error_with_status_404_when_get_company_given_company_name()
        {
            await ClearDataAsync();
            Company companyGiven = new Company("Blue");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("api/companies", SerializeObjectToContent(companyGiven));
            Company companyGiven2 = new Company("Sky");
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync("api/companies", SerializeObjectToContent(companyGiven2));
            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync("api/companies/Fly");

            Company? company = await DeserializeTo<Company>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage3.StatusCode);
        }
        [Fact]
        public async Task Should_return_a_pageSize_company_when_get_company_given_pageIndex_pageSize()
        {
            await ClearDataAsync();
            Company companyGiven = new Company("Blue");
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(companyGiven));
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Ski")));
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Skl")));
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Skk")));
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Syy")));
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Sfy")));

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync("api/companies/2/2");
            List<Company>? company = await DeserializeTo<List<Company>>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            Assert.Equal("Skl", company[0].Name);
            Assert.Equal("Skk", company[1].Name);
        }
        [Fact]
        public async Task Should_return_badrequest_when_get_company_list_given_pageIndex_pageSize()
        {
            await ClearDataAsync();
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Skk")));
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new Company("Ski")));

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync("api/companies/2/6");

            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage3.StatusCode);
        }

        [Fact]
        public async Task Should_return_badrequest_when_update_company_given_company_wrong_id()
        {
            await ClearDataAsync();
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new CreateCompanyRequest("Skk")));
            var httpMessage = await httpClient.PostAsync("api/companies", SerializeObjectToContent(new CreateCompanyRequest("Ski")));
            var company = await httpMessage.Content.ReadFromJsonAsync<Company>();

            HttpResponseMessage httpResponseMessage3 = await httpClient.PutAsJsonAsync($"api/companies/1234",new CreateCompanyRequest(company.Name));

            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage3.StatusCode);
        }

        [Fact]
        public async Task Should_return_updated_company_when_update_company_given_company_id()
        {
            await ClearDataAsync();
            await httpClient.PostAsync("api/companies", SerializeObjectToContent(new CreateCompanyRequest("Skk")));
            var httpResponseMessage = await httpClient.PostAsync("api/companies", SerializeObjectToContent(new CreateCompanyRequest("Ski")));
            var newCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            newCreated.Name = "SSkkii";
            HttpResponseMessage httpResponseMessage3 = await httpClient.PutAsJsonAsync($"api/companies/{newCreated.Id}", new CreateCompanyRequest(newCreated.Name));

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            var result = await httpResponseMessage3.Content.ReadFromJsonAsync<Company>();
            Assert.Equal(newCreated.Name,result.Name);

        }
        [Fact]
        public async Task Should_return_updated_employee_when_update_company_given_employee_name()
        {
            await ClearDataAsync();
            var httpResponseMessage = await httpClient.PostAsync("api/companies", SerializeObjectToContent(new CreateCompanyRequest("Ski")));
            var newCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            Employee employee = new Employee("worker", "1234");
            HttpResponseMessage httpResponseMessage3 = await httpClient.PostAsJsonAsync($"api/companies/{newCreated.Id}", employee);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            var result = await httpResponseMessage3.Content.ReadFromJsonAsync<Company>();
            Assert.Equal(employee.Name, result.Employee.Name);

        }

    }
}