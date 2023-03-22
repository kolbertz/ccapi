using CCApiLibrary.Interfaces;
using CCApiTestLibrary;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCProductPriceServiceTest
{
    public class ProductPricePoolIntegrationTest : ProductPriceTestBase, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
    {
        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productPricePoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPricePoolId = await PopulateSingleProductPricePool(dbConnection, "Bad Honnef", "Beschreibungstext");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/productpricepool/" + productPricePoolId);
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
            using (var services = application.Services.CreateScope())
            {
                HttpClient client = application.CreateClient();
                CreateBasicClientWithAuth(client);
                var response = await client.GetAsync("/api/v2/productpricepool/fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
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
                HttpResponseMessage response = await client.GetAsync("/api/v2/productpricepool");
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
                    Guid productPricePoolListOne, productPricePoolListTwo;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPricePoolListOne = await PopulateSingleProductPricePool(dbConnection, "Bad Honnef", "Bad Honnef Beschreibung");
                        productPricePoolListTwo = await PopulateSingleProductPricePool(dbConnection, "Köln", "Köln Beschreibung");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync("/api/v2/productpricepool");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    List<ProductPricePool> productPricePools = JsonConvert.DeserializeObject<List<ProductPricePool>>(await response.Content.ReadAsStringAsync());
                    Assert.Equal(2, productPricePools.Count);
                    Assert.Equal("Bad Honnef", productPricePools[0].Name[0].Text);
                    Assert.Equal("Köln Beschreibung", productPricePools[1].Description[0].Text);
                    Assert.Equal(StaticTestGuids.UserId, productPricePools[0].LastUpdatedUser);
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
                    Guid productPricePoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productPricePoolId = await PopulateSingleProductPricePool(dbConnection, "TestPool", "TestPool Beschreibung");
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync("/api/v2/productpricepool/" + productPricePoolId);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);   
                    ProductPricePool productPricePool = JsonConvert.DeserializeObject<ProductPricePool>(await response.Content.ReadAsStringAsync());
                    Assert.Equal("TestPool", productPricePool.Name[0].Text);
                    Assert.Equal("TestPool Beschreibung", productPricePool.Description[0].Text);
                    Assert.Equal(StaticTestGuids.UserId, productPricePool.CreatedUser);
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
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpricepool");
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            HttpClient client = application.CreateClient();
            HttpResponseMessage respone = await client.GetAsync("/api/v2/productpricepool");
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
                    ProductPricePoolBase productPricePoolBase = new ProductPricePoolBase
                    {
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Post Test")
                         },
                        Description = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Post Test Beschreibung")
                         },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId,
                        CurrencyId = StaticTestGuids.CurrencyId
                    };
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPricePoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("/api/v2/productpricepool/", httpContent);
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
                    ProductPricePoolBase productPricePoolBase = new ProductPricePoolBase
                    {
                        Description = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Post Test Beschreibung")
                         },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId,
                        CurrencyId = StaticTestGuids.CurrencyId
                    };
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPricePoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("/api/v2/productpricepool/", httpContent);
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
                    Guid productPricePoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await SetSystemSettingsId(dbConnection);
                        productPricePoolId = await PopulateSingleProductPricePool(dbConnection, "Put Test", "Put Test Beschreibung");
                    }
                    ProductPricePool productPricePoolBase = new ProductPricePool
                    {
                        Id = productPricePoolId,
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Name geändert")
                         },
                        Description = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Beschreibung geändert")
                         },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId,
                        CurrencyId = new Guid("25950bf7-ecf9-4589-9f55-df9307f490fd")
                    };
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPricePoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpricepool/" + productPricePoolId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PUT return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/productpricepool/" + productPricePoolId);
                        ProductPricePool pool = JsonConvert.DeserializeObject<ProductPricePool>(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal("Name geändert", pool.Name[0].Text);
                        Assert.Equal("Beschreibung geändert", pool.Description[0].Text);
                        Assert.Equal(new Guid("25950bf7-ecf9-4589-9f55-df9307f490fd"), pool.CurrencyId);
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
                    ProductPricePool productPricePoolBase = new ProductPricePool
                    {
                        Id = new Guid("25950bf7-ecf9-4589-9f55-df9307f490fd"),
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Name geändert")
                         },
                        Description = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Beschreibung geändert")
                         },
                        CurrencyId = StaticTestGuids.CurrencyId
                    };
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPricePoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpricepool/25950bf7-ecf9-4589-9f55-df9307f490fd", httpContent);
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
                    ProductPricePool productPricePoolBase = new ProductPricePool
                    {
                        Id = new Guid("25950bf7-ecf9-4589-9f55-df9307f490fd"),
                        Name = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Name geändert")
                         },
                        Description = new List<CCApiLibrary.Models.MultilanguageText>
                         {
                             new CCApiLibrary.Models.MultilanguageText("de-DE", "Beschreibung geändert")
                         },
                        SystemSettingsId = StaticTestGuids.SystemSettingsId,
                        CurrencyId = StaticTestGuids.CurrencyId
                    };
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPricePoolBase), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("/api/v2/productpricepool/25950bf7-ecf9-1111-1111-df9307f490fd", httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                }
                finally
                {
                    await ResetDatabaseAfterTesting(services);
                }
            }
        }
    }
}
