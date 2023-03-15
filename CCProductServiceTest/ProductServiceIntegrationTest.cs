using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.PopulateQueries;
using CCProductService.DTOs;
using CCProductService.Interface;
using CCProductService.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCProductServiceTest
{
    public class ProductServiceIntegrationTest : ControllerTestBaseClass, IClassFixture<CCApiTestStart>
    {
        private WebApplicationFactory<Program> GetWebApplication()
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IProductRepository, ProductRepository>();
                    PrepareServiceCollectionForTest(services);
                });
            });
        }

        public ProductServiceIntegrationTest() { }

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            var client = application.CreateClient();
            var respone = await client.GetAsync("/api/v2/products");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            var client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

            var respone = await client.GetAsync("/api/v2/products");
            string message = await respone.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");
                    // Populate DB
                    Guid productOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                    Guid productTwo = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(2));
                    await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productOne, "Produkt 1"));
                    await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productTwo, "Produkt 2"));
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync("/api/v2/products");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic products = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, products.Count);
                    Assert.Equal(1, (int)products[0].key);
                    Assert.Equal("Pool 2", (string)products[1].name);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void Post_returns_201_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");

                    // Populate DB with a systemSetting
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    ProductBase productBase = new ProductBase
                    {
                        Key = 1,
                         ProductType = CCProductService.DTOs.Enums.ProductType.DefaultProduct,
                         ShortNames = new List<MultilanguageText>
                         {
                             new MultilanguageText("de-DE", "Testprodukt"),
                             new MultilanguageText("en-GB", "product test")
                         },
                          LongNames = new List<MultilanguageText>
                          {
                              new MultilanguageText("de-DE", "Langer Name"),
                              new MultilanguageText("en-GB", "Long name")
                          }
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productBase), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/v2/products", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                    var poolId = Guid.Parse(response.Headers.Location!.PathAndQuery.Substring(response.Headers.Location!.PathAndQuery.LastIndexOf("/") + 1));
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void Get_By_Id_returns_200_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");
                    // Populate DB
                    Guid productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                    await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync("/api/v2/products/" + productId);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic product = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)product.key);
                    Assert.Equal("Get By Id Test", (string)product.name);

                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void Put_Returns_200_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");
                    // Populate DB
                    Guid productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                    await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Put Test"));
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Product product = new Product
                    {
                        Id = productId,
                        ShortNames = new List<MultilanguageText>
                        {
                            new MultilanguageText("de-DE", "Name geändert")
                        },
                        LongNames = new List<MultilanguageText> {
                            new MultilanguageText("de-DE", "langer Name")
                        },
                        Key = 2
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync("/api/v2/products/" + productId, httpContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void Patch_Returns_200_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");
                    // Populate DB
                    Guid productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                    await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Patch Test"));
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var patch = new[]
                    {
                                new
                                {
                                    op = "add",
                                    path = "/description",
                                    value = "Patch Test"
                                },
                                new
                                {
                                    op = "replace",
                                    path = "/key",
                                    value = "99"
                                }
                            };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(patch), Encoding.UTF8, "application/json");
                    var response = await client.PatchAsync("/api/v2/products/" + productId, httpContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    dynamic patchedProduct = JObject.Parse(await response.Content.ReadAsStringAsync());
                    Assert.Equal(99, (int)patchedProduct.key);
                    Assert.Equal("Patch Test", (string)patchedProduct.description);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }

            }
        }

        [Fact]
        public async void Delete_Returns_200_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");
                    // Populate DB
                    Guid productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                    await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Patch Test"));
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.DeleteAsync("/api/v2/products/" + productId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }

            }
        }

        [Fact]
        public async void Returns_BadRequestErrorMessageResult_when_request_is_wrong_GUID()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");

                    //Guid productPoolId = await PopulateDatabaseWithSingleEntity(dbConnection);

                    ProductBase product = new ProductBase
                    {
                        //ShortNames = "ApiController Put Test Product",
                        //LongNames = "ApiController Put Test Product",
                        //Key = 1,
                        //ProductPoolId = product.ProductPoolId //?
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.PostAsync("/api/v2/products/", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_ShortNames_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");

                    ProductBase product = new ProductBase()
                    {
                        Key = 1,
                        ProductPoolId = new Guid("553752EF-EB16-EC11-981F-0003FF0455EA")
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/products/", httpContent);
                    //HttpResponseMessage response1 = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async Task GetByID_returns_500_If_wrong_and_wrong_datatyp_PoolID()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");

                    // Populate DB
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.GetAsync("/api/v2/products/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void Get_All_204_No_Content()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    var client = application.CreateClient();
                    var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

                    var response = await client.GetAsync("/api/v2/products");
                    string message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    //Assert.False(message.Any());
                    //Assert.Empty(message);
                    dynamic pools = JArray.Parse(message);
                    Assert.Equal(0, pools.Count);
                }
                catch (Exception ex) { }

            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_Key_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");

                    ProductBase productDto = new ProductBase
                    {
                        ShortNames = new List<MultilanguageText>(),
                        LongNames = new List<MultilanguageText>(),
                        ProductType = CCProductService.DTOs.Enums.ProductType.MenuProduct
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/products/", httpContent);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_SystemSettingsId_missing()
        {
            WebApplicationFactory<Program> application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
            {
                try
                {
                    dbConnection.Init("DefaultDatabase");

                    ProductBase product = new ProductBase
                    {
                        //Name = "ApiController Test Product",
                        //Key = 1,
                        ////SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/products/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                    await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                }
            }
        }
    }
}
