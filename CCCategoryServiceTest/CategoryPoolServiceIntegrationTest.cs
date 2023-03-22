using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCCategoryService.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace CCCategoryServiceTest
{
    public class CategoryPoolServiceIntegrationTest : CategoryTestBase, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
    {
        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid categoryPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryPoolId = await PrepareDatabaseForTest(dbConnection, 0, "Warengruppe");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/categorypool/" + categoryPoolId);
                    var message = response.Content.ReadAsStringAsync();
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
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/categorypool/82a4252e-c58f-49d0-8476-b7e1a5fa4b11");
                    var message = response.Content.ReadAsStringAsync();
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
                var response = await client.GetAsync("/api/v2/categorypool/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
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
                HttpResponseMessage response = await client.GetAsync("/api/v2/categorypool");
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
                    Guid categoryPoolOne, categoryPoolTwo;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryPoolOne = await PrepareDatabaseForTest(dbConnection, 0, "Allergene");
                        categoryPoolTwo = await PrepareDatabaseForTest(dbConnection, 1, "Warengruppen", false);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync("/api/v2/categorypool");
                    string message = await response.Content.ReadAsStringAsync();
                    List<CategoryPool> categories = JsonConvert.DeserializeObject<List<CategoryPool>>(message);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(2, categories.Count);
                    Assert.Equal("Allergene", categories[0].Names[0].Text);
                    Assert.Equal("Beschreibungstext", categories[1].Descriptions[0].Text);
                    Assert.Equal(1, categories[1].Type);
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
                    Guid categoryPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryPoolId = await PrepareDatabaseForTest(dbConnection, 9, "Allergene");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync("/api/v2/categorypool/" + categoryPoolId);
                    CategoryPool categoryPool = JsonConvert.DeserializeObject<CategoryPool>(await response.Content.ReadAsStringAsync());
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal("Allergene", categoryPool.Names[0].Text);
                    Assert.Equal(9, categoryPool.Type);
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
            HttpResponseMessage respone = await client.GetAsync("/api/v2/categorypool");
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            HttpResponseMessage respone = await client.GetAsync("/api/v2/categorypool");
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
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemId(dbConnection);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    CategoryPoolBase categoryPoolBase = new CategoryPoolBase()
                    {
                        Type = 99,
                        Names = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Warengruppe")
                            },
                        Descriptions = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Beschreibung")
                            },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(categoryPoolBase), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/categorypool/", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
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
                    CategoryPoolBase categoryPoolBase = new CategoryPoolBase()
                    {
                        Names = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Warengruppe")
                            },
                        Descriptions = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Beschreibung")
                            },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(categoryPoolBase), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/categorypool/", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
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
                    Guid categoryPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        categoryPoolId = await PrepareDatabaseForTest(dbConnection, 10, "Warengruppe");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    CategoryPool categoryPoolBase = new CategoryPool()
                    {
                        Id = categoryPoolId,
                        Type = 99,
                        Names = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Allergene")
                            },
                        Descriptions = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Unverträglichkeiten")
                            },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(categoryPoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/categorypool/" + categoryPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PUT return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/categorypool/" + categoryPoolId);
                        CategoryPool pool = JsonConvert.DeserializeObject<CategoryPool>(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal(99, pool.Type);
                        Assert.Equal("Allergene", pool.Names[0].Text);
                        Assert.Equal("Unverträglichkeiten", pool.Descriptions[0].Text);
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
                    CategoryPool categoryPoolBase = new CategoryPool()
                    {
                        Id = new Guid("82a4252e-c58f-49d0-8476-b7e1a5fa4b11"),
                        Type = 99,
                        Names = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Allergene")
                            },
                        Descriptions = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Unverträglichkeiten")
                            },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(categoryPoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/categorypool/82a4252e-c58f-49d0-8476-b7e1a5fa4b11", httpContent);
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
                    CategoryPool categoryPoolBase = new CategoryPool()
                    {
                        Id = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeeb"),
                        Descriptions = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Unverträglichkeiten")
                            },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(categoryPoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/categorypool/fab8c985-6147-4eba-b2c7-5f7012c4aeeb", httpContent);
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
                    CategoryPool categoryPoolBase = new CategoryPool()
                    {
                        Id = new Guid("1118c985-6147-4eba-b2c7-5f7012c4aeeb"),
                        Type = 99,
                        Names = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Allergene")
                            },
                        Descriptions = new List<MultilanguageText>
                            {
                                new MultilanguageText("de-DE", "Unverträglichkeiten")
                            },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(categoryPoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/categorypool/fab8c985-6147-4eba-b2c7-5f7012c4aeeb", httpContent);
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
