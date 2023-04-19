using CCApiLibrary.Interfaces;
using CCApiLibrary.Models;
using CCApiTestLibrary.BaseClasses;
using CCApiTestLibrary.Interfaces;
using CCApiTestLibrary.PopulateQueries;
using CCProductService.DTOs;
using CCProductService.DTOs.Enums;
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
                    Guid productPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        // Populate DB
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        Guid productOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId), 1);
                        Guid productTwo = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(2, productPoolId));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productOne, "Produkt 1"));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productTwo, "Produkt 2"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync("/api/v2/product");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    List<ProductStandardPrice> products = JsonConvert.DeserializeObject<List<ProductStandardPrice>>(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, products.Count);
                    Assert.Equal(1, products[0].Key);
                    Assert.Equal(productPoolId, products[1].ProductPoolId);
                    Assert.Equal(ProductType.MenuProduct, products[0].ProductType);
                    Assert.Equal(ProductType.DefaultProduct, products[1].ProductType);
                    Assert.Equal("Produkt 2", products[1].ShortNames[0].Text);
                    Assert.Equal("Produkt 1 Lang", products[1].LongNames[0].Text);
                    Assert.Null(products[0].Standardprice);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                        ProductPoolId = default(Guid),
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
                    Guid productId, productPoolId;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId), 3);
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync($"/api/v2/product/{productId}");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    ProductBase product = JsonConvert.DeserializeObject<ProductBase>(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, product.Key);
                    Assert.Equal(productPoolId, product.ProductPoolId);
                    Assert.Equal("Get By Id Test", product.ShortNames[0].Text);
                    Assert.Equal("Get By Id Test Lang", product.LongNames[0].Text);
                    Assert.Equal(ProductType.Discount, product.ProductType);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductStrings());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                        Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Put Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    Product product = new Product
                    {
                        Id = productId,
                        ProductPoolId = default(Guid),
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                        Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                        Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
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
                        ProductPoolId = default(Guid),
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
                        ProductPoolId = default(Guid),
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
                        Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        for (int i = 0; i < 3; i++)
                        {
                            await dbConnection.ExecuteAsync(ProductQueries.SetProductBarcode(productId, "Barcode" + i));
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    }
                }
            }
        }

        [Fact]
        public async void ProductId_Get_Barcodes_return_404_if_product_exists_but_no_barcodes()
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
                        Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync($"/api/v2/product/{productId}/barcodes");
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProductBarcode());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    }
                }
            }
        }

        [Fact]
        public async void ProductId_Get_Pricings_return_200_And_PriceList()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId, productPricePoolId, productPriceListOne, productPriceListTwo, productPriceOne, productPriceTwo;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        Guid productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        productPricePoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateSingleProductPricePool("TestPool", "Beschreibung"));
                        productPriceListOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateSingleProductPriceList("TestList1", 1, 1));
                        productPriceListTwo = await dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateSingleProductPriceList("TestList2", 2, 2));
                        productPriceOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateProductPrice(productId, productPricePoolId, productPriceListOne));
                        productPriceTwo = await dbConnection.ExecuteScalarAsync<Guid>(ProductPriceQueries.PopulateProductPrice(productId, productPricePoolId, productPriceListTwo));
                        await dbConnection.ExecuteAsync(ProductPriceQueries.PopulateProductPriceDate(), param: new { priceId = productPriceOne, startDate = new DateTimeOffset(2023, 01, 01, 0, 0, 0, TimeSpan.FromMinutes(60)), value = 4.50m });
                        await dbConnection.ExecuteAsync(ProductPriceQueries.PopulateProductPriceDate(), param: new { priceId = productPriceOne, startDate = new DateTimeOffset(2099, 01, 01, 0, 0, 0, TimeSpan.FromMinutes(60)), value = 3.50m });
                        await dbConnection.ExecuteAsync(ProductPriceQueries.PopulateProductPriceDate(), param: new { priceId = productPriceTwo, startDate = new DateTimeOffset(2023, 02, 08, 0, 0, 0, TimeSpan.FromMinutes(60)), value = 1.00m });
                        await dbConnection.ExecuteAsync(ProductPriceQueries.PopulateProductPriceDate(), param: new { priceId = productPriceTwo, startDate = new DateTimeOffset(2099, 01, 01, 0, 0, 0, TimeSpan.FromMinutes(60)), value = 2.75m });
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    HttpResponseMessage response = await client.GetAsync($"/api/v2/product/{productId}/pricings?Datestring=" + DateTime.UtcNow.Date.ToLongDateString());
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    List<ProductPrice> productPrices = JsonConvert.DeserializeObject<List<ProductPrice>>(await response.Content.ReadAsStringAsync());
                    Assert.Equal(2, productPrices.Count());
                    Assert.Equal(4.50m, productPrices[0].Price);
                    Assert.Equal(new DateTimeOffset(2023, 01, 01, 0, 0, 0, TimeSpan.FromMinutes(60)), productPrices[0].StartDate);
                    Assert.Equal(1.00m, productPrices[1].Price);
                    Assert.Equal(new DateTimeOffset(2023, 02, 08, 0, 0, 0, TimeSpan.FromMinutes(60)), productPrices[1].StartDate);
                }
                finally
                {
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        await dbConnection.ExecuteAsync(ProductPriceQueries.DeleteProductPriceDates());
                        await dbConnection.ExecuteAsync(ProductPriceQueries.DeleteProductPrices());
                        await dbConnection.ExecuteAsync(ProductPriceQueries.DeleteProductPriceLists());
                        await dbConnection.ExecuteAsync(ProductPriceQueries.DeleteProductPricePools());
                        await dbConnection.ExecuteAsync(ProductQueries.DeleteProducts());
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                    }
                }
            }
        }
        [Fact]
        public async void GetProduct_PoolTypeCategory_200_and_Item_if_successfull() 
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId,categoryId, productPoolId, categoryPoolId, categoryStringOne, productCategoryOne;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        categoryPoolId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryPoolQueries.PopulateSingleCategoryPool("Warengruppen", 1));
                        categoryId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateSingleCategory(1, categoryPoolId));
                        categoryStringOne = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateCategoryStringsForSingleCategory(categoryId, "TestString"));
                        productCategoryOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductCategoryQueries.PopulateSingleCategory(productId, categoryId));
                        
                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync($"/api/v2/product/{productId}/categories");
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategoryStrings());
                        await dbConnection.ExecuteAsync(ProductCategoryQueries.DeleteProductCategories());
                    }
                }
            }

        }
        [Fact]
        public async void GetProduct_PoolTypeTags_200_and_Item_if_successfull()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId, categoryId, productPoolId, categoryPoolId, categoryStringOne, productCategoryOne;

                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        categoryPoolId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryPoolQueries.PopulateSingleCategoryPool("Essenskomponenten", 2));
                        categoryId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateSingleCategory(1, categoryPoolId));
                        categoryStringOne = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateCategoryStringsForSingleCategory(categoryId, "TestString"));
                        productCategoryOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductCategoryQueries.PopulateSingleCategory(productId, categoryId));

                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync($"/api/v2/product/{productId}/tags");
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategoryStrings());
                        await dbConnection.ExecuteAsync(ProductCategoryQueries.DeleteProductCategories());
                    }
                }
            }
        }
        [Fact]
        public async void GetProduct_PoolTypeTaxes_200_and_Item_if_successfull()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId, categoryId, productPoolId, categoryPoolId, categoryStringOne, productCategoryOne;

                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        categoryPoolId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryPoolQueries.PopulateSingleCategoryPool("Sondersteuer", 3));
                        categoryId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateSingleCategory(1, categoryPoolId));
                        categoryStringOne = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateCategoryStringsForSingleCategory(categoryId, "TestString"));
                        productCategoryOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductCategoryQueries.PopulateSingleCategory(productId, categoryId));

                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync($"/api/v2/product/{productId}/tags");
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategoryStrings());
                        await dbConnection.ExecuteAsync(ProductCategoryQueries.DeleteProductCategories());
                    }
                }
            }
        }
        [Fact]
        public async void GetProduct_PoolTypeMenuPlan_200_and_Item_if_successfull()
        {
            WebApplicationFactory<Program> application = GetWebApplication();
            using (IServiceScope services = application.Services.CreateScope())
            {
                try
                {
                    Guid productId, categoryId, productPoolId, categoryPoolId, categoryStringOne, productCategoryOne;
                    using (IApplicationDbConnection dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>())
                    {
                        dbConnection.Init(databaseKey);
                        productPoolId = await dbConnection.ExecuteScalarAsync<Guid>(ProductPoolQueries.PopulateSingleProductPool());
                        productId = await dbConnection.ExecuteScalarAsync<Guid>(ProductQueries.PopulateSingleProduct(1, productPoolId));
                        categoryPoolId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryPoolQueries.PopulateSingleCategoryPool("Allergene", 4));
                        categoryId = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateSingleCategory(1, categoryPoolId));
                        categoryStringOne = await dbConnection.ExecuteScalarAsync<Guid>(CategoryQueries.PopulateCategoryStringsForSingleCategory(categoryId, "TestString"));
                        productCategoryOne = await dbConnection.ExecuteScalarAsync<Guid>(ProductCategoryQueries.PopulateSingleCategory(productId, categoryId));

                        await dbConnection.ExecuteAsync(ProductQueries.PopulateProductStringsForSingleProduct(productId, "Get By Id Test"));
                    }
                    HttpClient client = application.CreateClient();
                    CreateBasicClientWithAuth(client);
                    var respone = await client.GetAsync($"/api/v2/product/{productId}/tags");
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
                        await dbConnection.ExecuteAsync(ProductPoolQueries.DeleteProductPools());
                        await dbConnection.ExecuteAsync(CategoryPoolQueries.DeleteCategoryPools());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategories());
                        await dbConnection.ExecuteAsync(CategoryQueries.DeleteCategoryStrings());
                        await dbConnection.ExecuteAsync(ProductCategoryQueries.DeleteProductCategories());
                    }
                }
            }
        }
    }
}
