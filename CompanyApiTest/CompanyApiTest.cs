using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;

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
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            Company companyGiven2 = new Company("RedSky Digital Media");

            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven2)
            );

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync(
                "/api/companies"
            );

            // Then
            List<Company>? companies1 = await DeserializeTo<List<Company>>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            Assert.Equal(companyGiven.Name, companies1[0].Name);
            Assert.Equal(companyGiven2.Name, companies1[1].Name);
        }

        [Fact]
        public async Task Should_return_one_company_when_get_given_companyName()
        {
            await ClearDataAsync();
            // Given
            var companyGiven = new CreateCompanyRequest { Name = "Google" };
           
            // When
            HttpResponseMessage createdResponseMessage = await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            HttpResponseMessage getResponseMessage = await httpClient.GetAsync("api/companies/Google");
            
            var company_ac3 = await DeserializeTo<Company>(getResponseMessage);

            //Then
            Assert.Equal(HttpStatusCode.OK, getResponseMessage.StatusCode);
            Assert.Equal("Google", company_ac3.Name);
        }

        [Fact]
        public async Task Should_return_404_when_get_company_given_not_existed_company_name()
        { 
            // Given 
            await ClearDataAsync();

            // When
            HttpResponseMessage getResponseMessage = await httpClient.GetAsync("/api/companies/Bluesky");
            var company = await DeserializeTo<Company>(getResponseMessage);

            //Then
            Assert.Equal(HttpStatusCode.NotFound, getResponseMessage.StatusCode);
        }

        // AC 4: As a user, I can obtain X(page size) companies from index of Y(page index start from 1)
        [Fact]
        public async Task Should_return_company_list_when_get_given_page_index_and_size()
        {
            await ClearDataAsync();
            // Given
            int pageIndex = 0;
            int pageSize = 2;

            // When
            Company companyGiven_ac4 = new Company("BlueSky Digital Media 1");

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven_ac4)
            );

            Company companyGiven2_ac4 = new Company("RedSky Digital Media 1");

            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven2_ac4)
            );

            HttpResponseMessage getResponseMessage_ac4 = await httpClient.GetAsync(
                $"/api/companies/{pageSize}/{pageIndex}"
            );

            List<Company>? companies_ac4 = await DeserializeTo<List<Company>>(getResponseMessage_ac4);

            //Then
            Assert.Equal(companyGiven2_ac4.Name, companies_ac4[1].Name);

        }

    }
}