using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Net;
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
        public async Task Should_return_created_company_with_status_201_when_create_company_given_a_company_name()
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

            // When
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven2)
            );

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync(
                "/api/companies"
            );

            // Then
            List<Company>? companies = await DeserializeTo<List<Company>>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            Assert.Equal(companyGiven.Name, companies[0].Name);
            Assert.Equal(companyGiven2.Name, companies[1].Name);
        }

        [Fact]
        public async Task Should_return_company_with_status_200_when_get_by_Id()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            Company? company = await DeserializeTo<Company>(httpResponseMessage);

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync(
                "/api/companies/" + company.Id
            );

            // Then
            Company? company3 = await DeserializeTo<Company>(httpResponseMessage3);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3.StatusCode);
            Assert.Equal(companyGiven.Name, company3.Name);
        }

        [Fact]
        public async Task Should_return_status_404_when_get_not_exist_company_by_name()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            HttpResponseMessage httpResponseMessage3 = await httpClient.GetAsync(
                "/api/companies/BlueSky"
            );

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage3.StatusCode);
        }

        [Fact]
        public async Task Should_return_company_with_status_200_when_get_by_pageInfo()
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

            Company companyGiven3 = new Company("GreenSky Digital Media");

            HttpResponseMessage httpResponseMessage3 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven3)
            );

            HttpResponseMessage httpResponseMessage4 = await httpClient.GetAsync(
                "/api/companies/1/1"
            );

            // Then
            List<Company>? company = await DeserializeTo<List<Company>>(httpResponseMessage4);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage4.StatusCode);
            Assert.Equal(companyGiven2.Name, company[0].Name);
        }

        [Fact]
        public async Task Should_return_changed_company_with_status_200_when_put()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            Company? company = await DeserializeTo<Company>(httpResponseMessage);

            Company newCompany = new Company("RedSky Digital Media");

            HttpResponseMessage httpResponseMessage2 = await httpClient.PutAsync(
                "/api/companies/" + company.Id,
                SerializeObjectToContent(newCompany)
            );

            // Then
            Company? company2 = await DeserializeTo<Company>(httpResponseMessage2);

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage2.StatusCode);
            Assert.Equal(newCompany.Name, company2.Name);
        }

        [Fact]
        public async Task Should_return_created_employee_with_status_201_when_create_employee()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            Employee employeeGiven = new Employee("Employee1", 100);
            Company? company = await DeserializeTo<Company>(httpResponseMessage);

            // When
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync(
                "/api/companies/" + company.Id + "/employees",
                SerializeObjectToContent(employeeGiven)
            );

            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage2.StatusCode);
            Employee? employeeCreated = await DeserializeTo<Employee>(httpResponseMessage2);
            Assert.NotNull(employeeGiven);
            Assert.NotNull(employeeGiven.Id);
            Assert.Equal(employeeGiven.Name, employeeCreated.Name);
        }

        [Fact]
        public async Task Should_return_204_when_delete_employee()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            Employee employeeGiven = new Employee("Employee1", 100);
            Company? companyCreated = await DeserializeTo<Company>(httpResponseMessage);

            // When
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync(
                "/api/companies/" + companyCreated.Id + "/employees",
                SerializeObjectToContent(employeeGiven)
            );

            Employee? employeeCreated = await DeserializeTo<Employee>(httpResponseMessage2);

            HttpResponseMessage httpResponseMessage3 = await httpClient.DeleteAsync(
                "/api/companies/" + companyCreated.Id + "/employees/" + employeeCreated.Id
            );

            // Then
            Assert.Equal(HttpStatusCode.NoContent, httpResponseMessage3.StatusCode);
        }

        [Fact]
        public async Task Should_return_all_employees_when_get_all_employees()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            Employee employeeGiven = new Employee("Employee1", 100);
            Company? company = await DeserializeTo<Company>(httpResponseMessage);

            // When
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsync(
                "/api/companies/" + company.Id + "/employees",
                SerializeObjectToContent(employeeGiven)
            );

            Employee employeeGiven2 = new Employee("Employee2", 200);
            HttpResponseMessage httpResponseMessage3 = await httpClient.PostAsync(
                "/api/companies/" + company.Id + "/employees",
                SerializeObjectToContent(employeeGiven2)
            );

            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage2.StatusCode);
            Employee? employeeCreated = await DeserializeTo<Employee>(httpResponseMessage2);
            Employee? employeeCreated2 = await DeserializeTo<Employee>(httpResponseMessage3);

            HttpResponseMessage httpResponseMessage4 = await httpClient.GetAsync(
                "/api/companies/" + company.Id + "/employees"
            );

            List<Employee>? employeeListCreated = await DeserializeTo<List<Employee>>(httpResponseMessage4);

            Assert.Equal(employeeGiven.Name, employeeListCreated[0].Name);
            Assert.Equal(employeeGiven2.Name, employeeListCreated[1].Name);
        }
    }
}