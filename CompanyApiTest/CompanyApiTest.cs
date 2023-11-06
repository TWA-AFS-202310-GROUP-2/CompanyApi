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
            // Given
            await ClearDataAsync();
            Company companyGiven_ac2 = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage_ac2 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven_ac2)
            );

            Company companyGiven2_ac2 = new Company("RedSky Digital Media");

            HttpResponseMessage httpResponseMessage2_ac2 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven2_ac2)
            );

            HttpResponseMessage httpResponseMessage3_ac2 = await httpClient.GetAsync(
                "/api/companies"
            );

            // Then
            List<Company>? companies_ac2 = await httpResponseMessage3_ac2.Content.ReadFromJsonAsync<List<Company>>();
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage3_ac2.StatusCode);
            Assert.Equal(companyGiven_ac2.Name, companies_ac2[0].Name);
            Assert.Equal(companyGiven2_ac2.Name, companies_ac2[1].Name);
        }

        [Fact]
        public async Task Should_return_one_company_when_get_given_companyName()
        {
            await ClearDataAsync();
            // Given
            Company companyGiven = new Company("Google");
           
            // When
            HttpResponseMessage createdResponseMessage = await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            HttpResponseMessage getResponseMessage = await httpClient.GetAsync("api/companies/Google");
            Company company_ac3 = await getResponseMessage.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Equal(HttpStatusCode.OK, getResponseMessage.StatusCode);
            Assert.Equal("Google", company_ac3.Name);
        }

        [Fact]
        public async Task Should_return_404_when_get_company_given_not_existed_company_Id()
        { 
            // Given 
            await ClearDataAsync();

            HttpResponseMessage getResponseMessage = await httpClient.GetAsync("/api/companies/Bluesky");
            Company? companyCreated = await getResponseMessage.Content.ReadFromJsonAsync<Company>();
            
            //When
            HttpResponseMessage getHttpResponseMessage = await httpClient.GetAsync(
                $"/api/companies/randomId");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, getHttpResponseMessage.StatusCode);
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

        // AC 5: As a user, I can update basic information of an existing company
        [Fact]
        public async Task Should_return_200_when_update_an_exist_company()
        {
            // Given
            await ClearDataAsync();

            Company companyGiven_ac5 = new Company("BlueSky_ac5");

            HttpResponseMessage createHttpResponseMessage_ac5 = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven_ac5)
            );
            Company? companyCreated = await createHttpResponseMessage_ac5.Content.ReadFromJsonAsync<Company>();
            // When
            companyCreated.Name = "ChangedName";
            HttpResponseMessage putHttpResponseMessage = await httpClient.PutAsJsonAsync(
                $"/api/companies/{companyCreated.Id}", companyCreated);

            // Then
            Assert.Equal(HttpStatusCode.OK, putHttpResponseMessage.StatusCode);
            Company? company = await putHttpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(company);
            Assert.Equal(companyCreated.Name, company.Name);
        }

        // AC 6: As a user, I can add an employee to a specific company
        [Fact]
        public async Task Should_return_created_employee_with_status_201_given_an_employee_and_Company()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media ac6");
            HttpResponseMessage createHttpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            Company? companyCreated = await createHttpResponseMessage.Content.ReadFromJsonAsync<Company>();

            // When
            string createEmployeeUrl = $"/api/companies/{companyCreated?.Id}/employees";
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest("Tom", 10000);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(createEmployeeUrl, employeeRequest);
            HttpResponseMessage getCompanyHttpResponseMessage = await httpClient.GetAsync($"/api/companies/{companyCreated?.Id}");

            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Employee? employee = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();
            Company? company = await getCompanyHttpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(employee);
            Assert.NotNull(employee.Id);
            Assert.Equal(employeeRequest.Name, employee.Name);
            Assert.Equal(employeeRequest.Salary, employee.Salary);
        }

        [Fact]
        public async Task Should_return_created_employee_with_status_404_given_an_employee_with_Company_non_exist()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");
            HttpResponseMessage createHttpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            await createHttpResponseMessage.Content.ReadFromJsonAsync<Company>();

            // When
            string createEmployeeUrl = $"/api/companies/wrongCompanyId/employees";
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest("Tom", 10000);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(createEmployeeUrl, employeeRequest);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }
        
        // AC 7: As a user, I can delete a specific employee under a specific company
        [Fact]
        public async Task Should_return_status_204_when_delete_employee_successfully()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");
            HttpResponseMessage createCompantResMsg = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            Company? companyCreated = await createCompantResMsg.Content.ReadFromJsonAsync<Company>();
            string employeeUrl = $"/api/companies/{companyCreated?.Id}/employees";
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest("Jim", 10000);
            HttpResponseMessage createEmployeeResMsg = await httpClient.PostAsJsonAsync(employeeUrl, employeeRequest);

            // When
            Employee? employee = await createEmployeeResMsg.Content.ReadFromJsonAsync<Employee>();
            HttpResponseMessage deleteEmployeeResMsg = await httpClient.DeleteAsync(employeeUrl + $"/{employee?.Id}");
            HttpResponseMessage getCompantResMsg = await httpClient.GetAsync($"/api/companies/{companyCreated?.Id}");
            Company? company = await getCompantResMsg.Content.ReadFromJsonAsync<Company>();

            // Then
            Assert.Equal(HttpStatusCode.NoContent, deleteEmployeeResMsg.StatusCode);
            Assert.Equal(0, company?.Employees.Count());
        }

        [Fact]
        public async Task Should_return_status_404_when_delete_employee_given_wrong_employeeId()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");
            HttpResponseMessage createCompantResMsg = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            Company? companyCreated = await createCompantResMsg.Content.ReadFromJsonAsync<Company>();
            string employeeUrl = $"/api/companies/{companyCreated?.Id}/employees";
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest("Tom", 10000);
            HttpResponseMessage createEmployeeResMsg = await httpClient.PostAsJsonAsync(employeeUrl, employeeRequest);

            // When
            Employee? employee = await createEmployeeResMsg.Content.ReadFromJsonAsync<Employee>();
            HttpResponseMessage deleteEmployeeResMsg = await httpClient.DeleteAsync(employeeUrl + $"/wrongEmployeeId");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, deleteEmployeeResMsg.StatusCode);
        }

        [Fact]
        public async Task Should_return_status_404_when_delete_employee_given_wrong_companyId()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");
            HttpResponseMessage createCompantResMsg = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            Company? companyCreated = await createCompantResMsg.Content.ReadFromJsonAsync<Company>();
            string employeeUrl = $"/api/companies/{companyCreated?.Id}/employees";
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest("Tom", 10000);
            HttpResponseMessage createEmployeeResMsg = await httpClient.PostAsJsonAsync(employeeUrl, employeeRequest);

            // When
            Employee? employee = await createEmployeeResMsg.Content.ReadFromJsonAsync<Employee>();
            HttpResponseMessage deleteEmployeeResMsg = await httpClient.DeleteAsync($"/api/companies/wrongCompanyId/employees/{employee.Id}");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, deleteEmployeeResMsg.StatusCode);
        }

    }
}