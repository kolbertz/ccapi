//using CCApiLibrary.Interfaces;
//using CCApiTestLibrary.BaseClasses;
//using CCCategoryService.Dtos;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Net;
//using System.Text;

//namespace CCApiTest.Category
//{
//    public class CategoryControllerTests : CategoryControllerBase, IClassFixture<CCApiTestStart>
//    {

//        public CategoryControllerTests() { }

//        [Fact]
//        public async void Get_returns_401_Unauthorized_if_not_authenticated()
//        {
//            var application = GetWebApplication();

//            var client = application.CreateClient();
//            var response = await client.GetAsync("/api/v2/category");
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async void Get_returns_200_Authorized()
//        {
//            var application = GetWebApplication();
//            var client = application.CreateClient();
//            var response = await client.GetAsync("/api/v2/category");
//            string message = await response.Content.ReadAsStringAsync();
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        }

//        [Fact]
//        public async void Get_All_returns_200_And_List_if_sucessful()
//        {
//            var application = GetWebApplication();
//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabase");

//                    await PopulateDatabaseWithList(dbConnection);
//                    var client = CreateBasicClientWithAuth(application);
//                    var response = await client.GetAsync("/api/v2/category");
//                    string message = await response.Content.ReadAsStringAsync();
//                    dynamic categorys = JArray.Parse(message);
//                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//                    Assert.Equal(2, categorys.Count);
//                    Assert.Equal(1, (int)categorys[0].key);
//                    Assert.Equal("Pool 2", (string)categorys[1].name);
//                }
//                catch (Exception)
//                {


//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }
//        }

//        [Fact]
//        public async void Post_all_returns_201_if_sucessful()
//        {
//            var application = GetWebApplication();
//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabase");

//                    // Populate DB with a systemSetting
//                    await PopulateDbWithSystemSetting(dbConnection);
//                    var client = CreateBasicClientWithAuth(application);

//                    CategoryPoolBase productPool = new CategoryPoolBase
//                    {
//                        Description = "ApiController Test Category",
//                        Name = "ApiController Test Category",
//                        Key = 1,
//                        SystemSettingsId = systemSettingsId
//                    };
//                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(productPool), Encoding.UTF8, "application/json");

//                    var response = await client.PostAsync("/api/v2/category", httpContent);
//                    var message = await response.Content.ReadAsStringAsync();
//                    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

//                    var poolId = Guid.Parse(response.Headers.Location!.PathAndQuery.Substring(response.Headers.Location!.PathAndQuery.LastIndexOf("/") + 1));
//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }

//        }
//        [Fact]
//        public async void Get_By_Id_return_200_And_Item_if_sucessful()
//        {
//            var application = GetWebApplication();
//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabse");

//                    Guid categoryId = await PopulateDatabaseWithSingleEntity(dbConnection);
//                    var client = CreateBasicClientWithAuth(application);
//                    var response = await client.GetAsync("/api/v2/category" + categoryId);
//                    string message = await response.Content.ReadAsStringAsync();
//                    dynamic category = JObject.Parse(message);
//                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//                    Assert.Equal(1, (int)category.key);
//                    Assert.Equal("Category 1", (string)category.name);
//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }
//        }

//        [Fact]
//        public async void Put_Returns_200_if_successful()
//        {
//            var application = GetWebApplication();
//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabase");

//                    Guid categoryId = await PopulateDatabaseWithSingleEntity(dbConnection);
//                    var client = CreateBasicClientWithAuth(application);
//                    CategoryBase category = new CategoryBase
//                    {

//                    };
//                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
//                    var response = await client.PutAsync("/api/v2/product/" + categoryId, httpContent);
//                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//                    dynamic patchedPool = JObject.Parse(await response.Content.ReadAsStringAsync());
//                    Assert.Equal(99, (int)patchedPool.Key);
//                    Assert.Equal(systemSettingsId, (Guid)patchedPool.systemSettingsId);
//                    Assert.Equal("Patch Test", (string)patchedPool.description);
//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }
//        }

//        [Fact]
//        public async void Delete_Returns_200_if_successful()
//        {
//            var application = GetWebApplication();
//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabase");

//                    Guid categoryId = await PopulateDatabaseWithSingleEntity(dbConnection);
//                    var client = CreateBasicClientWithAuth(application);
//                    var response = await client.DeleteAsync("/api/v2/category/ + categoryId");
//                    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }
//        }

//        [Fact]
//        public async void Returns_BadRequestErrorMessageResult_when_request_is_wrong_GUID()
//        {
//            var application = GetWebApplication();
//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabase");

//                    CategoryBase category = new CategoryBase
//                    {

//                    };
//                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
//                    var client = CreateBasicClientWithAuth(application);
//                    var response = await client.PostAsync("/api/v2/product/", httpContent);

//                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }
//        }
//        [Fact]
//        public async void NotValidMode_400_Rquired_Field_name_missing()
//        {
//            var application = GetWebApplication();

//            using (var services = application.Services.CreateScope())
//            {
//                IApplicationDbConnection dbConnection = null;
//                try
//                {
//                    dbConnection = services.ServiceProvider.GetService<IApplicationDbConnection>();
//                    dbConnection.Init("TestDatabase");

//                    CategoryBase category = new CategoryBase { };
//                    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
//                    var client = CreateBasicClientWithAuth(application);

//                    var response = await client.PutAsync("/api/v2/product/", httpContent);

//                    Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
//                }
//                finally
//                {
//                    await DePopulateDatabase(dbConnection);
//                }
//            }
//        }
//    }
//}
