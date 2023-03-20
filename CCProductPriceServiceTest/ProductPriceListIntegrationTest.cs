using CCApiLibrary.Interfaces;
using CCApiTestLibrary;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCProductPriceServiceTest
{
    public class ProductPriceListIntegrationTest : ProductPriceTestBase, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
    {
        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPriceListId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPriceListId = await PopulateSingleProductPriceList(dbConnection, "Gästeliste", 1, 1);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/productpricelist/" + productPriceListId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void GetByID_returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                var response = await client.GetAsync("/api/v2/productpricelist/fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async void Get_All_204_No_Content()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                HttpResponseMessage response = await client.GetAsync("/api/v2/productpricelist");
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
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPriceListOne, productPriceListTwo;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPriceListOne = await PopulateSingleProductPriceList(dbConnection, "Gästeliste", 10, 2);
                        productPriceListTwo = await PopulateSingleProductPriceList(dbConnection, "Mitarbeiterliste", 1, 1);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync("/api/v2/productpricelist");
                    string message = await response?.Content.ReadAsStringAsync();
                    List<ProductPriceList> priceLists = JsonConvert.DeserializeObject<List<ProductPriceList>>(message);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(2, priceLists.Count);
                    Assert.Equal("Gästeliste", priceLists[0].Name[0].Text);
                    Assert.Equal(1, priceLists[1].Key);
                    Assert.Equal(2, priceLists[0].Priority);
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
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPriceListId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPriceListId = await PopulateSingleProductPriceList(dbConnection, "Azubiliste", 99, 5);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync("/api/v2/productpricelist/" + productPriceListId);
                    string message = await response.Content.ReadAsStringAsync();
                    ProductPriceList productPriceList = JsonConvert.DeserializeObject<ProductPriceList>(message);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal("Azubiliste", productPriceList.Name[0].Text);
                    Assert.Equal(99, productPriceList.Key);
                    Assert.Equal(5, productPriceList.Priority);
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
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpricelist");
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpricelist");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        public async void Patch_Returns_204_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
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
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                    }
                    HttpClient client = application.CreateClient(); 
                    CreateBasicClientWithAuth(client);
                    ProductPriceListBase productPrice = new ProductPriceListBase
                    {
                        Key = 88,
                        Name = new List<CCApiLibrary.Models.MultilanguageText> {
                            new CCApiLibrary.Models.MultilanguageText("de-DE", "Gästeliste")
                        },
                        Priority = 3,
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPrice), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/productpricelist/", httpContent);
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
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPriceListBase productPrice = new ProductPriceListBase
                    {
                        Name = new List<CCApiLibrary.Models.MultilanguageText> {
                            new CCApiLibrary.Models.MultilanguageText("de-DE", "Gästeliste")
                        },
                        Priority = 3,
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPrice), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/v2/productpricelist/", httpContent);
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
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPriceListId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPriceListId = await PopulateSingleProductPriceList(dbConnection, "Mitarbeiterliste", 12, 2);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPriceList productPriceList = new ProductPriceList
                    {
                        Id = productPriceListId,
                        Key = 99,
                        Priority = 5,
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                           {
                               new CCApiLibrary.Models.MultilanguageText("de-DE", "Gästeliste")
                           },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPriceList), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpricelist/" + productPriceListId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PUT return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/productpricelist/" + productPriceList);
                        ProductPriceList productPrice = JsonConvert.DeserializeObject<ProductPriceList>(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal(99, productPrice.Key);
                        Assert.Equal(5, productPrice.Priority);
                        Assert.Equal("Gästeliste", productPrice.Name[0].Text);
                    }
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
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPriceListId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPriceListId = await PopulateSingleProductPriceList(dbConnection, "Mitarbeiterliste", 12, 2);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPriceList productPriceList = new ProductPriceList
                    {
                        Id = productPriceListId,
                        Key = 99,
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                           {
                               new CCApiLibrary.Models.MultilanguageText("de-DE", "Gästeliste")
                           },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPriceList), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpricelist/" + productPriceListId, httpContent);
                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }

        [Fact]
        public async void Returns_BadRequestErrorMessageResult_when_route_Id_and_Model_Id_are_different()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPriceListId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPriceListId = await PopulateSingleProductPriceList(dbConnection, "Mitarbeiterliste", 12, 2);
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductPriceList productPriceList = new ProductPriceList
                    {
                        Id = new Guid("1118c985-6147-4eba-b2c7-5f7012c4aeeb"),
                        Key = 99,
                        Priority = 5,
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                           {
                               new CCApiLibrary.Models.MultilanguageText("de-DE", "Gästeliste")
                           },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPriceList), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpricelist/" + productPriceListId, httpContent);
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
