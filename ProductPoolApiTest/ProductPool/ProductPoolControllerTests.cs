using CCProductPoolService.Data;
using CCProductPoolService.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductPoolApiTest.Interface;
using ProductPoolApiTest.ProductPool;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace ProductPoolApiTest
{
    public class ProductPoolControllerTests : ProductPoolControllerBase, IClassFixture<ProductPoolStart>
    {
        public ProductPoolControllerTests() {}

        [Fact]
        public async void Get_returns_401_Unauthorized_if_not_authenticated()
        {
            var application = GetWebApplication();

            var client = application.CreateClient();
            var respone = await client.GetAsync("/api/v2/productpool");
            Assert.Equal(HttpStatusCode.Unauthorized, respone.StatusCode);
        }

        [Fact]
        public async void Get_returns_200_Authorized()
        {
            var application = GetWebApplication();
            var client = application.CreateClient();
            var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

            var respone = await client.GetAsync("/api/v2/productpool");
            string message = await respone.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
        }

        [Fact] 
        public async void Get_All_returns_200_And_List_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                AramarkDbProduction20210816Context ctx = null;
                try
                {
                    ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();
                    // Populate DB
                    await PopulateDatabaseWithList(transaction, ctx);
                    transaction = null;
                    var client = CreateClientWithAuth(application);
                    var respone = await client.GetAsync("/api/v2/productpool");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic pools = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, pools.Count);
                    Assert.Equal(1, (int)pools[0].key);
                    Assert.Equal("Pool 2", (string)pools[1].name);
                }
                finally
                {
                    await DePopulateDatabase(transaction, ctx);
                    transaction = null;
                }
            }
        }

        [Fact]
        public async void Post_returns_201_if_successful()
        {
            var application = GetWebApplication();
             
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                AramarkDbProduction20210816Context ctx = null;
                try
                {
                    ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();

                    // Populate DB with a systemSetting
                    await PopulateDbWithSystemSetting(ctx, transaction, true);
                    transaction = null;
                    var client = CreateClientWithAuth(application);
                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Description = "ApiController Test Pool",
                        Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/v2/productpool", httpContent);
                    var message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

                    var poolId = Guid.Parse(response.Headers.Location!.PathAndQuery.Substring(response.Headers.Location!.PathAndQuery.LastIndexOf("/") + 1));
                }
                finally
                {
                    await DePopulateDatabase(transaction, ctx);
                    transaction = null;
                }
            }
        }

        [Fact]
        public async void Get_By_Id_returns_200_And_Item_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                AramarkDbProduction20210816Context ctx = null;
                try
                {
                    ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(transaction, ctx);
                    transaction = null;
                    var client = CreateClientWithAuth(application);
                    var respone = await client.GetAsync("/api/v2/productpool/" + productPoolId);
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic pool = JObject.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(1, (int)pool.key);
                    Assert.Equal("Pool 1", (string)pool.name);

                }
                finally
                {
                    await DePopulateDatabase(transaction, ctx);
                    transaction = null;
                }
            }
        }

        [Fact]
        public async void Put_Returns_200_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                AramarkDbProduction20210816Context ctx = null;
                try
                {
                    ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(transaction, ctx);
                    transaction = null;
                    var client = CreateClientWithAuth(application);
                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Id = productPoolId,
                        Description = "ApiController Put Test Pool",
                        Name = "ApiController Put Test Pool",
                        Key = 2,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync("/api/v2/productpool/" + productPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(transaction, ctx);
                    transaction = null;
                }

            }
        }

        [Fact]
        public async void Patch_Returns_200_And_Item_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                AramarkDbProduction20210816Context ctx = null;
                try
                {
                    ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(transaction, ctx);
                    transaction = null;
                    var client = CreateClientWithAuth(application);
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
                    var response = await client.PatchAsync("/api/v2/productpool/" + productPoolId, httpContent);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    dynamic patchedPool = JObject.Parse(await response.Content.ReadAsStringAsync());
                    Assert.Equal(99, (int)patchedPool.key);
                    Assert.Equal(systemSettingsId, (Guid)patchedPool.systemSettingsId);
                    Assert.Equal("Patch Test", (string)patchedPool.description);
                }
                finally
                {
                    await DePopulateDatabase(transaction, ctx);
                    transaction = null;
                }

            }
        }

        [Fact]
        public async void Delete_Returns_200_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IDbContextTransaction transaction = null;
                AramarkDbProduction20210816Context ctx = null;
                try
                {
                    ctx = services.ServiceProvider.GetService<AramarkDbProduction20210816Context>();
                    transaction = ctx.Database.BeginTransaction();
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(transaction, ctx);
                    transaction = null;
                    var client = CreateClientWithAuth(application);
                    var response = await client.DeleteAsync("/api/v2/productpool/" + productPoolId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(transaction, ctx);
                    transaction = null;
                }

            }
        }
    }
}
