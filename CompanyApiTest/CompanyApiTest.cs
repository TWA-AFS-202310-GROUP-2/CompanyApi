using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Xml.Linq;

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
            Company companyGiven = new Company("BlueSky Digital Media");
            
            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies", 
                SerializeObjectToContent(companyGiven)
            );
           
            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Company? companyCreated = await DeserializeTo<Company>(httpResponseMessage);
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

        [Fact]
        public async Task Should_return_all_companied_when_get_given_no_company()
        {
            //Given
            await ClearDataAsync();
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("api/companies");
            List<Company> companyCreated = await DeserializeTo<List<Company>>(httpResponseMessage);
            //then
            Assert.Equal(0, companyCreated.Count);
        }

        [Fact]
        public async Task Should_return_all_companied_when_get_given_an_company()
        {
            //Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");
            await httpClient.PostAsync("/api/companies", SerializeObjectToContent(companyGiven));
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("api/companies");
            List<Company> allcompany = await DeserializeTo<List<Company>>(httpResponseMessage);
            //then
            Assert.Equal(companyGiven.Name, allcompany[0].Name);
        }

        [Fact]
        public async Task Should_return_an_selected_company_When_get_Given_an_name()
        {
            //Given
            await ClearDataAsync();
            string name = "test by id";
            Company givencompany = new Company(name);
            await httpClient.PostAsync("/api/companies", SerializeObjectToContent(givencompany));
            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"api/companies/{name}");
            Company selectedcompany = await DeserializeTo<Company>(httpResponseMessage);
            //then
            Assert.Equal(givencompany.Name, selectedcompany.Name);
        }

        [Fact]
        public async Task Should_return_pagesize_companies_from_page_index_When_get_Given_x_size_y_index()
        {
            //Given
            await ClearDataAsync();
            int pageIndex = 5;
            int pageSize = 2;
            GenerateCompanies(pageIndex, pageSize);
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"api/companies/page?pageIndex={pageIndex}&pageSize={pageSize}");
            List <Company> companiesOnPageIndex = await DeserializeTo<List<Company>>(httpResponseMessage);
            //then
            Assert.Equal("new company 9", companiesOnPageIndex[0].Name);
            Assert.Equal("new company 10", companiesOnPageIndex[1].Name);
        }

        private static StringContent SerializeObjectToContent<T>(T objectGiven)
        {
            return new StringContent(JsonConvert.SerializeObject(objectGiven), Encoding.UTF8, "application/json");
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

        private async Task ClearDataAsync()
        {
            await httpClient.DeleteAsync("/api/companies");
        }
    }
}