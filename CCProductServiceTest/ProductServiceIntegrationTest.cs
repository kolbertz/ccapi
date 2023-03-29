using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCApiTestLibrary.PopulateQueries;
using CCProductService.DTOs;
using CCProductService.Interface;
using CCProductService.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Net;
using System.Text;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace CCProductServiceTest
{
    public class ProductServiceIntegrationTest : ControllerTestBaseClass, IServiceIntegrationTestBase, IClassFixture<CCApiTestStart>
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
            var respone = await client.GetAsync("/api/v2/product");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            var client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

            var respone = await client.GetAsync("/api/v2/product");
            string message = await respone.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact]
        public async void Get_All_returns_200_And_List_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        Guid productOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                        Guid productTwo = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(2));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productOne, "Produkt 1"));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productTwo, "Produkt 2"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync("/api/v2/product");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic products = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, products.Count);
                    Assert.Equal(1, (int)products[0].key);
                    Assert.Equal("Produkt 2", (string)products[1].shortNames[0].text);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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

                    var response = await client.PostAsync("/api/v2/product", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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
                    Guid productId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync("/api/v2/product/" + productId);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic product = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)product.key);
                    Assert.Equal("Get By Id Test", (string)product.shortNames[0].text);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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
                    Guid productId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Put Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Product product = new Product
                    {
                        Id = productId,
                        ProductType = CCProductService.DTOs.Enums.ProductType.MenuProduct,
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
                    var response = await client.PutAsync("/api/v2/product/" + productId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
                }
            }
        }

        //[Fact]
        public async void Patch_Returns_204_And_Item_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Patch Test"));
                    }
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
                    HttpResponseMessage response = await client.PatchAsync("/api/v2/product/" + productId, httpContent);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                    // if PATCH return successful status code, proove it by sending a corresponding GET request
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        HttpResponseMessage getResponse = await client.GetAsync("/api/v2/product/" + productId);
                        dynamic patchedProduct = JObject.Parse(await getResponse.Content.ReadAsStringAsync());
                        Assert.Equal(99, (int)patchedProduct.key);
                        Assert.Equal("Patch Test", (string)patchedProduct.description);
                    }
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
                }
            }
        }

        [Fact]
        public async void Delete_Returns_204_if_successful()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Patch Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.DeleteAsync("/api/v2/product/" + productId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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
                    Product product = new Product
                    {
                        Id = new Guid("6bbd2f72-94a9-453b-aa28-cff702e8fa4a"),
                        ProductType = CCProductService.DTOs.Enums.ProductType.MenuProduct,
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
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var response = await client.PutAsync("/api/v2/product/126e57fa-d9ec-40e3-ae51-8480e9cd9c05", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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
                var response = await client.GetAsync("/api/v2/product/fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
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
                var response = await client.GetAsync("/api/v2/product");
                string message = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                dynamic pools = JArray.Parse(message);
                Assert.Equal(0, pools.Count);
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
                    ProductBase productDto = new ProductBase
                    {
                        ShortNames = new List<MultilanguageText>(),
                        LongNames = new List<MultilanguageText>(),
                        ProductType = CCProductService.DTOs.Enums.ProductType.MenuProduct
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PostAsync("/api/v2/product/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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
                    Product productBase = new Product
                    {
                        Id = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeeb"),
                        Key = 1,
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
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);

                    var response = await client.PutAsync("/api/v2/product/fab8c985-6147-4eba-b2c7-5f7012c4aeeb", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
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
                    Product product = new Product
                    {
                        Id = new Guid("82a4252e-c58f-49d0-8476-b7e1a5fa4b11"),
                        ProductType = CCProductService.DTOs.Enums.ProductType.MenuProduct,
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
                    var response = await client.PutAsync("/api/v2/product/82a4252e-c58f-49d0-8476-b7e1a5fa4b11", httpContent);
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
                }
            }
        }

        [Fact]
        public async void Delete_Returns_404_If_given_Id_not_found()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.DeleteAsync("/api/v2/product/82a4252e-c58f-49d0-8476-b7e1a5fa4b11");
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async void ProductId_Get_Barcodes_return_200_And_Barcode_List()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1));
                        for (int i = 0; i < 3; i++)
                        {
                            await dbConnection.ExecuteAsync(ProductQueries.SetPriductBarcode(productId, "Barcode" + i));
                        }
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync($"/api/v2/product/{productId}/barcodes");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    List<string> barcodes = JsonConvert.DeserializeObject<List<string>>(await  response.Content.ReadAsStringAsync());
                    Assert.Equal(3, barcodes.Count);
                    Assert.Equal("Barcode1", barcodes[1]);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductBarcode());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                    }
                }
            }
        }
    }
}
