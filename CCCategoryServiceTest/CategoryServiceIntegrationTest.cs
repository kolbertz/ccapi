using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCCategoryService.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCCategoryServiceTest
{
    public class CategoryServiceIntegrationTest : CategoryTestBase, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
    {
        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    (Guid categoryId, Guid categoryPoolId) Ids;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        Ids = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.DeleteAsync("/api/v2/category/" + Ids.categoryId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Delete_Returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                var response = await client.DeleteAsync("/api/v2/category/82a4252e-c58f-49d0-8476-b7e1a5fa4b11");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async void GetByID_returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                var response = await client.GetAsync("/api/v2/category/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async void Get_All_204_No_Content()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                HttpResponseMessage response = await client.GetAsync("/api/v2/category");
                string message = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                dynamic pools = JArray.Parse(message);
                Assert.Equal(0, pools.Count);
            }
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    (Guid categoryId, Guid categoryPoolId) IdOne, IdTwo;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        IdOne = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                        IdTwo = await PopulateCategory(dbConnection, "Heißgetränke", 2, 0, "Getränke", false);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/category");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic categories = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, categories.Count);
                    Assert.Equal("Glutenfrei", (string)categories[0].categoryNames[0].text);
                    Assert.Equal(2, (int)categories[1].categoryKey);
                    Assert.Equal(IdOne.categoryId, (Guid)categories[0].id);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Get_By_Id_returns_200_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    (Guid categoryId, Guid categoryPoolId) Ids;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        Ids = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage respone = await client.GetAsync("/api/v2/category/" + Ids.categoryId);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic category = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)category.categoryKey);
                    Assert.Equal("Glutenfrei", (string)category.categoryNames[0].text);
                    Assert.Equal(Ids.categoryId, (Guid)category.id);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            CreateBasicClientWithAuth(client);
            HttpResponseMessage respone = await client.GetAsync("/api/v2/category");
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            HttpResponseMessage respone = await client.GetAsync("/api/v2/category");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        public async void Patch_Returns_204_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {

                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Post_returns_201_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid categoryPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryPoolId = await PrepareDatabaseForTest(dbConnection, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    CategoryBase category = new CategoryBase
                    {
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Glutenfrei")
                        },
                        CategoryKey = 15,
                        CategoryPoolId = categoryPoolId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/category", httpContent);
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Post_Returns_422_if_required_prop_is_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    CategoryBase category = new CategoryBase
                    {
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Glutenfrei")
                        },
                        CategoryKey = 15
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("/api/v2/category", httpContent);
                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Put_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    (Guid categoryId, Guid categoryPoolId) Ids;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        Ids = await PopulateCategory(dbConnection, "Glutenfrei", 1, 0, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Category category = new Category
                    {
                        Id = Ids.categoryId,
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Ungesund")
                        },
                        CategoryKey = 15,
                        CategoryPoolId = Ids.categoryPoolId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/category/" + Ids.categoryId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PATCH return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/category/" + Ids.categoryId);
                        dynamic result = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal(15, (int)result.categoryKey);
                        Assert.Equal("Ungesund", (string)result.categoryNames[0].text);
                    }
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Put_Returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Category category = new Category
                    {
                        Id = new Guid("82a4252e-c58f-49d0-8476-b7e1a5fa4b11"),
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Ungesund")
                        },
                        CategoryKey = 15,
                        CategoryPoolId = new Guid("f56af880-55b0-4b03-ae44-8dc72ccdeff3")
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/category/82a4252e-c58f-49d0-8476-b7e1a5fa4b11", httpContent);
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Put_Returns_422_if_required_prop_is_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Category category = new Category
                    {
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Ungesund")
                        },
                        CategoryKey = 15,
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/category/fab8c985-6147-4eba-b2c7-5f7012c4aeeb", httpContent);
                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Put_Returns_BadRequestErrorMessageResult_when_route_Id_and_Model_Id_are_different()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Category category = new Category
                    {
                        Id = new Guid("dbd0ce34-ea1f-437d-8719-920c615fd6f9"),
                        CategoryNames = new List<MultilanguageText>()
                        {
                            new MultilanguageText("de-DE", "Ungesund")
                        },
                        CategoryKey = 15,
                        CategoryPoolId = new Guid("6748e96a-27e0-4573-a907-a49c941b2a25")
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/category/6748e96a-27e0-4573-a907-a49c941b2a25", httpContent);
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }
    }
}
