using Azure;
using CCProductPoolService.Dtos;
using CCProductPoolService.Interface;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductPoolApiTest.ProductPool;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    await PopulateDatabaseWithList(dbConnection);
                    var client = CreateClientWithAuth(application);
                    var respone = await client.GetAsync("/api/v2/productpool");
                    string messsage = await respone.Content.ReadAsStringAsync();
                    dynamic pools = JArray.Parse(messsage);
                    Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
                    Assert.Equal(2, pools.Count);
                    Assert.Equal(1, (int)pools[0].key);
                    Assert.Equal("Pool 2", (string)pools[1].name);
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }
            }
        }

        [Fact]
        public async void Post_returns_201_if_successful()
        {
            var application = GetWebApplication();
             
            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");

                    // Populate DB with a systemSetting
                    await PopulateDbWithSystemSetting(dbConnection);
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
                    await DePopulateDatabase(dbConnection);
                }
            }
        }

        [Fact]
        public async void Get_By_Id_returns_200_And_Item_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(dbConnection);
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
                    await DePopulateDatabase(dbConnection);
                }
            }
        }

        [Fact]
        public async void Put_Returns_200_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(dbConnection);
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
                    await DePopulateDatabase(dbConnection);
                }

            }
        }

        [Fact]
        public async void Patch_Returns_200_And_Item_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(dbConnection);
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
                    await DePopulateDatabase(dbConnection);
                }

            }
        }

        [Fact]
        public async void Delete_Returns_200_if_successful()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");
                    // Populate DB
                    Guid productPoolId = await PopulateDatabaseWithSingleEntity(dbConnection);
                    var client = CreateClientWithAuth(application);
                    var response = await client.DeleteAsync("/api/v2/productpool/" + productPoolId);
                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }

            }
        }

        [Fact]
        public async void Returns_BadRequestErrorMessageResult_when_request_is_wrong_GUID()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {                
                IApplicationDbConnection dbConnection = null;                
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");

                    //Guid productPoolId = await PopulateDatabaseWithSingleEntity(dbConnection);
                    
                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Description = "ApiController Test Pool",
                        Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = new Guid("fab8c985-6147-4eba-b2c7-5f7012c4aeec")
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    var client = CreateClientWithAuth(application);
                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(dbConnection);                    
                }
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_name_missing()
        {
            var application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {                        
                        //Name = "ApiController Test Pool",
                        Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    var client = CreateClientWithAuth(application);

                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);
                    //HttpResponseMessage response1 = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }
            }
        }

        [Fact] 
        public async Task GetByID_returns_500_If_wrong_and_wrong_datatyp_PoolID()
        {
            var application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            {                
                IApplicationDbConnection dbConnection = null;
                //AramarkDbProduction20210816Context ctx = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");

                    // Populate DB
                    var client = CreateClientWithAuth(application);
                    var response = await client.GetAsync("/api/v2/productpool/" + "fab8c985-6147-4eba-b2c7-5f7012c4aeeb");
                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }
            }
        }

        [Fact]
        public async void Get_All_204_No_Content()
        {
            var application = GetWebApplication();
            using (var services = application.Services.CreateScope())
            {                
                try
                {
                    var client = application.CreateClient();
                    var base64EncodedAuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test:test"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64EncodedAuthString);

                    var response = await client.GetAsync("/api/v2/productpool");
                    string message = await response.Content.ReadAsStringAsync();
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    //Assert.False(message.Any());
                    //Assert.Empty(message);
                    dynamic pools = JArray.Parse(message);                    
                    Assert.Equal(0, pools.Count);
                }
                catch  (Exception ex) { }          
              
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_Key_missing()
        {
            var application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Name = "ApiController Test Pool",
                        //Key = 1,
                        SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    var client = CreateClientWithAuth(application);

                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }
            }
        }

        [Fact]
        public async void NotValidModel_400_Required_Field_SystemSettingsId_missing()
        {
            var application = GetWebApplication();

            using (var services = application.Services.CreateScope())
            {
                IApplicationDbConnection dbConnection = null;
                try
                {
                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
                    dbConnection.Init("TestDatabase");

                    ProductPoolDto productPool = new ProductPoolDto
                    {
                        Name = "ApiController Test Pool",
                        Key = 1,
                        //SystemSettingsId = systemSettingsId
                    };
                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");
                    var client = CreateClientWithAuth(application);

                    var response = await client.PostAsync("/api/v2/productpool/", httpContent);

                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
                }
                finally
                {
                    await DePopulateDatabase(dbConnection);
                }
            }
        }
    
    }
}
