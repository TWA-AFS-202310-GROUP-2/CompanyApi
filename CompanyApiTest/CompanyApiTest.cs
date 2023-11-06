using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi.Any;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace CompanyApiTest
{
    public class CompanyApiTest
    {
        private readonly HttpClient httpClient;
        private readonly WebApplicationFactory<Program> webApplicationFactory;

        public CompanyApiTest()
        {
            webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Should_return_created_company_with_status_201_when_create_company_given_a_company_name()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            
            // When
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
           
            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            Assert.Equal(companyGiven.Name, companyCreated.Name);
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
        [Fact]
        public async Task Should_return_bad_request_when_create_company_given_an_existed_company_name()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };

            // When
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_all_companies_when_get_companies_given_all()
        {
            // Given
            await ClearDataAsync();
            var companyOne = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var companyTwo = new CreateCompanyRequest
            {
                Name = "ThoughtWorks"
            };
            await httpClient.PostAsJsonAsync("/api/companies", companyOne);
            await httpClient.PostAsJsonAsync("/api/companies", companyTwo);

            // When
            var httpResponseMessage = await httpClient.GetAsync("/api/companies");
        
            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var companies = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();
            Assert.NotNull(companies);
            Assert.Equal(2, companies.Count);
        }

        [Fact]
        public async Task Should_return_company_when_get_company_given_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);

            // When
            httpResponseMessage = await httpClient.GetAsync($"/api/companies/{companyCreated.Id}");
           
            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var company = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(company);
            Assert.Equal(companyCreated.Id, company.Id);
            Assert.Equal(companyCreated.Name, company.Name);
        }

        [Fact]
        public async Task Should_return_not_found_when_get_company_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);


            // When
            httpResponseMessage = await httpClient.GetAsync($"/api/companies/{Guid.NewGuid().ToString()}");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_lists_when_get_companies_given_page_and_page_size()
        {
            // Given
            await ClearDataAsync();
            var companyOne = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var companyTwo = new CreateCompanyRequest
            {
                Name = "ThoughtWorks"
            };
            await httpClient.PostAsJsonAsync("/api/companies", companyOne);
            await httpClient.PostAsJsonAsync("/api/companies", companyTwo);

            // When
            var httpResponseMessage = await httpClient.GetAsync("/api/companies?pageIndex=1&pageSize=1");

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var companies = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();
            Assert.NotNull(companies);
            Assert.Single(companies);
        }

        [Fact]
        public async Task Should_return_empty_list_when_get_companies_given_not_existed_page_and_size()
        {
            // Given
            await ClearDataAsync();
            var companyOne = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var companyTwo = new CreateCompanyRequest
            {
                Name = "ThoughtWorks"
            };
            await httpClient.PostAsJsonAsync("/api/companies", companyOne);
            await httpClient.PostAsJsonAsync("/api/companies", companyTwo);

            // When
            var httpResponseMessage = await httpClient.GetAsync("/api/companies?pageIndex=3&pageSize=1");

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var companies = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();
            Assert.NotNull(companies);
            Assert.Empty(companies);
        }

        [Fact]
        public async Task Should_return_empty_list_when_get_companies_given_negative_page_and_size()
        {
            // Given
            await ClearDataAsync();
            var companyOne = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var companyTwo = new CreateCompanyRequest
            {
                Name = "ThoughtWorks"
            };
            await httpClient.PostAsJsonAsync("/api/companies", companyOne);
            await httpClient.PostAsJsonAsync("/api/companies", companyTwo);

            // When
            var httpResponseMessage = await httpClient.GetAsync("/api/companies?pageIndex=-1&pageSize=-1");

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var companies = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();
            Assert.NotNull(companies);
            Assert.Empty(companies);
        }

        [Fact]
        public async Task Should_return_updated_company_when_update_company_given_company_id_and_new_info()
        {   
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            var companyUpdateRequest = new UpdateCompanyRequest
            {
                Name = "ThoughtWorks"
            };

            // When
            httpResponseMessage = await httpClient.PutAsJsonAsync($"/api/companies/{companyCreated.Id}", companyUpdateRequest);

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var companyUpdated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyUpdated);
            Assert.Equal(companyCreated.Id, companyUpdated.Id);
            Assert.Equal(companyUpdateRequest.Name, companyUpdated.Name);
        }

        [Fact]
        public async Task Should_return_not_found_when_update_company_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyUpdateRequest = new UpdateCompanyRequest
            {
                Name = "ThoughtWorks"
            };

            // When
            var httpResponseMessage = await httpClient.PutAsJsonAsync($"/api/companies/{Guid.NewGuid().ToString()}", companyUpdateRequest);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }
        [Fact]
        public async Task Should_return_created_employee_with_status_201_when_create_employee_given_a_company_id_and_employee_info()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            var employeeGiven = new CreateEmployeeRequest
            {
                Name = "Tom",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };

            // When
            httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated.Id}/employees", employeeGiven);

            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            var employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();
            Assert.NotNull(employeeCreated);
            Assert.NotNull(employeeCreated.Id);
            Assert.Equal(employeeGiven.Name, employeeCreated.Name);
            Assert.Equal(employeeGiven.Salary, employeeCreated.Salary);
            Assert.Equal(companyCreated.Id, employeeCreated.CompanyId);
        }
        [Fact]
        public async Task Should_return_not_found_when_create_employee_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var employeeGiven = new CreateEmployeeRequest
            {
                Name = "Tom",
                Salary = 10000,
                CompanyId = Guid.NewGuid().ToString()
            };

            // When
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{employeeGiven.CompanyId}/employees", employeeGiven);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_request_when_update_company_given_company_id_and_new_info_with_unknown_field()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            StringContent content = new StringContent("{\"unknownField\": \"ThoughtWorks\"}", Encoding.UTF8, "application/json");

            // When
            httpResponseMessage = await httpClient.PutAsync($"/api/companies/{companyCreated.Id}", content);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_not_found_when_delete_employee_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{Guid.NewGuid().ToString()}/employees/{Guid.NewGuid().ToString()}");

            // When
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_not_found_when_delete_employee_given_not_existed_employee_id_but_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);

            // When
            httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{companyCreated.Id}/employees/{Guid.NewGuid().ToString()}");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_no_content_when_delete_employee_given_existed_employee_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            var employeeGiven = new CreateEmployeeRequest
            {
                Name = "Tom",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };
            httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated.Id}/employees", employeeGiven);
            var employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();
            Assert.NotNull(employeeCreated);
            Assert.NotNull(employeeCreated.Id);
            var companyAll = await httpClient.GetFromJsonAsync<List<Company>>("/api/companies");

            // When
            httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{companyCreated.Id}/employees/{employeeCreated.Id}");

            // Then
            Assert.Equal(HttpStatusCode.NoContent, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_all_employees_when_get_all_employees_given_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };

            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);

            var employeeOne = new CreateEmployeeRequest
            {
                Name = "Tom",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };
            var employeeTwo = new CreateEmployeeRequest
            {
                Name = "Jerry",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };
            await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated.Id}/employees", employeeOne);
            await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated.Id}/employees", employeeTwo);

            // When
            httpResponseMessage = await httpClient.GetAsync($"/api/companies/{companyCreated.Id}/employees");

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var employees = await httpResponseMessage.Content.ReadFromJsonAsync<List<Employee>>();
            Assert.NotNull(employees);
            Assert.Equal(2, employees.Count);
        }

        [Fact]
        public async Task Should_return_not_found_when_get_all_employees_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();

            // When
            var httpResponseMessage = await httpClient.GetAsync($"/api/companies/{Guid.NewGuid().ToString()}/employees");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_updated_employee_when_update_employee_given_company_id_and_employee_id_and_new_info()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };

            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);

            var employeeGiven = new CreateEmployeeRequest
            {
                Name = "Tom",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };
            httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated.Id}/employees", employeeGiven);
            var employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();
            Assert.NotNull(employeeCreated);
            Assert.NotNull(employeeCreated.Id);

            var employeeUpdateRequest = new UpdateEmployeeRequest
            {
                Name = "Jerry",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };

            // When
            httpResponseMessage = await httpClient.PutAsJsonAsync($"/api/companies/{companyCreated.Id}/employees/{employeeCreated.Id}", employeeUpdateRequest);

            // Then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            var employeeUpdated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();
            Assert.NotNull(employeeUpdated);
            Assert.Equal(employeeCreated.Id, employeeUpdated.Id);
            Assert.Equal(employeeUpdateRequest.Name, employeeUpdated.Name);
            Assert.Equal(employeeUpdateRequest.Salary, employeeUpdated.Salary);
            Assert.Equal(employeeUpdateRequest.CompanyId, employeeUpdated.CompanyId);
        }

        [Fact]
        public async Task Should_return_not_found_when_update_employee_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var employeeUpdateRequest = new UpdateEmployeeRequest
            {
                Name = "Jerry",
                Salary = 10000,
                CompanyId = Guid.NewGuid().ToString()
            };

            // When
            var httpResponseMessage = await httpClient.PutAsJsonAsync($"/api/companies/{employeeUpdateRequest.CompanyId}/employees/{Guid.NewGuid().ToString()}", employeeUpdateRequest);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_not_found_when_update_employee_given_not_existed_employee_id_but_existed_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };

            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);

            var employeeUpdateRequest = new UpdateEmployeeRequest
            {
                Name = "Jerry",
                Salary = 10000,
                CompanyId = companyCreated.Id
            };

            // When
            httpResponseMessage = await httpClient.PutAsJsonAsync($"/api/companies/{companyCreated.Id}/employees/{Guid.NewGuid().ToString()}", employeeUpdateRequest);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_no_content_when_delete_company_given_company_id()
        {
            // Given
            await ClearDataAsync();
            var companyGiven = new CreateCompanyRequest
            {
                Name = "BlueSky Digital Media"
            };

            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
            var companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);

            // When
            httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{companyCreated.Id}");

            // Then
            Assert.Equal(HttpStatusCode.NoContent, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_not_found_when_delete_company_given_not_existed_company_id()
        {
            // Given
            await ClearDataAsync();

            // When
            var httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{Guid.NewGuid().ToString()}");

            // Then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }

        private async Task ClearDataAsync()
        {
            await httpClient.DeleteAsync("/api/companies");
        }
    }
}
